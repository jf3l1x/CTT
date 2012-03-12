using System;
using System.Linq;
using CTT.Controllers;
using CTT.Models;
using MvcContrib.PortableAreas;
using OpenIdPortableArea.Helpers;
using OpenIdPortableArea.Messages;
using Raven.Client;

namespace CTT.Services
{
    public class AuthenticatedMessageHandler : MessageHandler<AuthenticatedMessage>
    {
        public override void Handle(AuthenticatedMessage message)
        {
            using (IDocumentSession session = RavenController.DocumentStore.OpenSession())
            {
                User user = session.Query<User>().FirstOrDefault(x => x.Email == message.ClaimsResponse.Email);
                if (user == null)
                {
                    user = new User
                               {
                                   Email = message.ClaimsResponse.Email,
                                   FullName = message.ClaimsResponse.FullName,
                                   NickName = message.ClaimsResponse.Nickname
                               };
                }
                if (!session.Query<User>().Any(x => x.IsAdmin))
                {
                    user.IsAdmin = true;
                }
                session.Store(user);
                session.SaveChanges();
                OpenIdHelpers.Login(user.Email, user.Email, TimeSpan.FromHours(1), false);
                
            }
        }
    }
}