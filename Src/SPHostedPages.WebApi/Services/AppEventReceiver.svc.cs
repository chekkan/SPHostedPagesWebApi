using System;
using System.Linq;
using Microsoft.SharePoint.Client.EventReceivers;
using SPHostedPages.WebApi.Models;

namespace SPHostedPages.WebApi.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            var result = new SPRemoteEventResult();

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    HandleAppInstalled(properties);
                    break;
                case SPRemoteEventType.AppUninstalling:
                    HandleAppUninstalling(properties);
                    break;
            }

            return result;
        }

        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            var appdomainurl = properties.AppEventProperties.AppWebFullUrl.GetComponents(UriComponents.SchemeAndServer,
                UriFormat.SafeUnescaped);
            System.Diagnostics.Trace.WriteLine(string.Format("Adding the app host domain url to db: {0}", appdomainurl));
            // write this value to the database
            var context = new MyContext();
            // only add the domain if it doesnt already exist. because it is the primary key
            // the app uninstall should have remove the domain url
            if (
                context.AppDomains.FirstOrDefault(
                    ad => ad.HostUrl.Equals(appdomainurl)) == null)
            {
                context.AppDomains.Add(new Models.AppDomain()
                {
                    HostUrl = appdomainurl
                });
                context.SaveChanges();
                System.Diagnostics.Trace.WriteLine(string.Format("Added the app host domain url to db: {0}",
                    appdomainurl));
            }
        }

        private void HandleAppUninstalling(SPRemoteEventProperties properties)
        {
            var appdomainurl = properties.AppEventProperties.AppWebFullUrl.GetComponents(UriComponents.SchemeAndServer,
                UriFormat.SafeUnescaped);
            System.Diagnostics.Trace.WriteLine(string.Format("Removing the app host domain url to db: {0}", appdomainurl));
            // remove this value from the database
            var context = new MyContext();
            context.AppDomains.Remove(
                context.AppDomains.FirstOrDefault(
                    ad => ad.HostUrl == appdomainurl));
            context.SaveChanges();
            System.Diagnostics.Trace.WriteLine(string.Format("Removed the app host domain url to db: {0}", appdomainurl));
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

    }
}
