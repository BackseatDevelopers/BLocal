using System;
using System.Web.Mvc;
using BLocal.Core;

namespace BLocal.Web
{
    public static class HtmlExtentions
    {
        /// <summary>
        /// Provides Localization Helper. Requires availability of LocalizationRepository on MVC DependencyResolver
        /// </summary>
        /// <param name="helper">Current HTML Helper</param>
        /// <param name="abSegment">Segment for A/B Testing if applicable</param>
        public static LocalizationHelper Local(this HtmlHelper helper, string abSegment = null)
        {
            var context =
                DependencyResolver.Current.GetService<ILocalizationContext>()
                ?? BLocalConfiguration.LocalizationContext;

            if (context == null)
                throw new Exception("Please set up the DependencyResolver for ILocalizationContext, or provide one on BLocalConfiguration.LocizationContext");
            
            return new LocalizationHelper(helper, context.DebugMode, new RepositoryWrapper(context.Repository, context.Repository.DefaultPart, abSegment));
        }

        /// <summary>
        /// Provides Localization Helper.
        /// </summary>
        /// <param name="helper">Current HTML Helper</param>
        /// <param name="repository">Repository to draw localizations from</param>
        /// <param name="debug">Whether or not to allow debug mode</param>
        /// <param name="abSegment">Segment for AB testing (null for no ab testing)</param>
        /// <returns></returns>
        public static LocalizationHelper Local(this HtmlHelper helper, LocalizationRepository repository, bool debug, string abSegment = null)
        {
            return new LocalizationHelper(helper, debug, new RepositoryWrapper(repository, repository.DefaultPart, abSegment));
        }
    }

    public interface ILocalizationContext
    {
        LocalizationRepository Repository { get; }
        bool DebugMode { get; }
    }

    public class AlwaysDebuggingContext : ILocalizationContext
    {
        public AlwaysDebuggingContext(LocalizationRepository repository)
        {
            Repository = repository;
        }

        public LocalizationRepository Repository { get; private set; }
        public bool DebugMode { get { return true; } }
    }

    public class NeverDebuggingContext : ILocalizationContext
    {
        public NeverDebuggingContext(LocalizationRepository repository)
        {
            Repository = repository;
        }

        public LocalizationRepository Repository { get; private set; }
        public bool DebugMode { get { return false; } }
    }
    public class ManualConfigurationContext : ILocalizationContext
    {
        public ManualConfigurationContext(LocalizationRepository repository, bool debugMode)
        {
            Repository = repository;
            DebugMode = debugMode;
        }

        public LocalizationRepository Repository { get; private set; }
        public bool DebugMode { get; private set; }
    }
}