using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using CTT.Controllers;
using Raven.Client;
using Telerik.Web.Mvc.UI;

namespace CTT.Models.Menu
{
    public class MenuCreator
    {
        private readonly IPrincipal _user;
        private IList<MenuItemEntry> _menu;

        public MenuCreator(IPrincipal user)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                _menu = HttpContext.Current.Session[ItemKeys.MenuKey] as IList<MenuItemEntry>;
                if (_menu == null)
                {
                    _menu = new List<MenuItemEntry>();
                    XElement xe = XElement.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("CTT.Models.Menu.Menu.xml"));
                    foreach (XElement menuItem in xe.Nodes())
                    {
                        MenuItemEntry entry = LoadEntry(menuItem);
                        if (entry != null && entry.HasValue)
                        {
                            _menu.Add(entry);
                        }
                    }
                    HttpContext.Current.Session[ItemKeys.MenuKey] = _menu;

                }
            }
            _user = user;
        }
        protected IList<MenuItemEntry> Menu
        {
            get
            {
                return _menu;
            }
        }
        private string GetSafeNullValue(XAttribute xa)
        {
            if (xa != null)
            {
                return xa.Value;
            }
            return string.Empty;
        }
        private bool GetSafeBoolValue(XAttribute xa)
        {
            if (xa != null)
            {
                return bool.Parse(xa.Value);
            }
            return false;
        }
        private MenuItemEntry LoadEntry(XElement menuItem)
        {
            var retval = new MenuItemEntry() { Text = GetSafeNullValue(menuItem.Attribute("Text")), ImageURL = GetSafeNullValue(menuItem.Attribute("ImageURL")), URL = GetSafeNullValue(menuItem.Attribute("DestinationURL")), AdminOnly = GetSafeBoolValue(menuItem.Attribute("AdminOnly")) };

            foreach (XElement subItem in menuItem.Nodes())
            {
                MenuItemEntry entry = LoadEntry(subItem);
                if (entry != null && entry.HasValue)
                {
                    retval.SubItems.Add(entry);
                }
            }
            return retval;
        }
        public void CreateMenu(MenuItemFactory menu)
        {
            if (_user.Identity.IsAuthenticated)
            {
                using (IDocumentSession store = RavenController.DocumentStore.OpenSession())
                {
                    User user = store.Query<User>().FirstOrDefault(x => x.Email == _user.Identity.Name);
                    if (user != null)
                    {
                        foreach (MenuItemEntry mi in this.Menu)
                        {
                            if (!mi.AdminOnly || (mi.AdminOnly && user.IsAdmin))
                            {
                                MenuItemBuilder mib = menu.Add();
                                mib.Text(mi.Text);
                                if (!string.IsNullOrEmpty(mi.URL))
                                {
                                    mib.Url(mi.URL);
                                }
                                if (!string.IsNullOrEmpty(mi.ImageURL))
                                {
                                    mib.ImageUrl(mi.ImageURL);
                                }
                                mib.Items(items =>
                                              {
                                                  FillMenuItem(items, mi, null);
                                              });
                            }
                        }
                    }
                }
            }
        }
        private void FillMenuItem(MenuItemFactory factory, MenuItemEntry mie, params object[] parameters)
        {
            foreach (MenuItemEntry mi in mie.SubItems)
            {
                MenuItemEntry applied = mi.ApplyParameters(parameters);
                MenuItemBuilder mib = factory.Add();
                mib.Text(applied.Text);
                if (!string.IsNullOrEmpty(applied.URL))
                {
                    mib.Url(applied.URL);
                }
                if (!string.IsNullOrEmpty(applied.ImageURL))
                {
                    mib.ImageUrl(applied.ImageURL);
                }
                mib.Items(items => FillMenuItem(items, applied, parameters));
            }
        }
    }

    public class MenuItemEntry
    {
        private static readonly Regex regex = new Regex(@"\{[^}]+\}",
                                                        RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace |
                                                        RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string _text;

        public MenuItemEntry()
        {
            SubItems = new List<MenuItemEntry>();
        }

        public string URL { get; set; }
        public string ImageURL { get; set; }
        public IList<MenuItemEntry> SubItems { get; set; }
        public bool AdminOnly { get; set; }

        public string Text
        {
            get
            {
                string retval = strings.ResourceManager.GetString(_text);
                if (retval == null)
                {
                    retval = _text;
                }
                return retval;
            }
            set { _text = value; }
        }

        public bool HasValue
        {
            get { return SubItems.Count > 0 || !string.IsNullOrEmpty(URL); }
        }

        public MenuItemEntry ApplyParameters(params object[] parameters)
        {
            MenuItemEntry retval = this;
            if (parameters != null && parameters.Length > 0)
            {
                retval = new MenuItemEntry
                             {
                                 URL = Format(URL, parameters),
                                 ImageURL = Format(ImageURL, parameters),
                                 SubItems = SubItems,
                                 Text = Format(Text, parameters)
                             };
            }
            return retval;
        }

        private string Format(string value, params object[] parameters)
        {
            if (regex.Match(value).Success)
            {
                return string.Format(value, parameters);
            }
            return value;
        }
    }
}