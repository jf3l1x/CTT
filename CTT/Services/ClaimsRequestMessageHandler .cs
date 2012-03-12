using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using MvcContrib.PortableAreas;
using OpenIdPortableArea.Messages;

namespace CTT.Services
{
    public class ClaimsRequestMessageHandler : MessageHandler<ClaimsRequestMessage>
    {
        public override void Handle(ClaimsRequestMessage message)
        {
            message.Claim.FullName = DemandLevel.Require;
            message.Claim.Email = DemandLevel.Require;
            message.Claim.Nickname = DemandLevel.Require;
        }
    }
}