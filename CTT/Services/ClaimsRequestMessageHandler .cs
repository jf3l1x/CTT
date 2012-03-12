using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcContrib.PortableAreas;
using OpenIdPortableArea.Messages;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

namespace CTT.Services
{
    public class ClaimsRequestMessageHandler:MessageHandler<ClaimsRequestMessage>
    {
        public override void Handle(ClaimsRequestMessage message)
        {
            message.Claim.FullName=DemandLevel.Require;
            message.Claim.Email = DemandLevel.Require;
            message.Claim.Nickname = DemandLevel.Require;
        }
    }
}