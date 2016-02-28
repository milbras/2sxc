﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.Environment.Interfaces;
using ToSic.SexyContent.Search;
using ToSic.SexyContent.Statics;

namespace ToSic.SexyContent
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone & App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    public class SexyContent 
    {
        #region Properties
        /// <summary>
        /// The Content Data Context pointing to a full EAV
        /// </summary>
        public EavDataController ContentContext;

        internal int? ZoneId { get; set; }

        public int? AppId { get; private set; }

        public Templates Templates { get; internal set; }

		public ContentGroups ContentGroups { get; internal set; }

        private App _app;
        public App App {
            get { return _app ?? (_app = AppHelpers.GetApp(ZoneId.Value, AppId.Value, PortalSettingsOfOriginalModule)); }
        }

        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal PortalSettings PortalSettingsOfOriginalModule { get; set; }

        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public Environment.Environment Environment = new Environment.Environment();

        #endregion


        #region SexyContent Constructor


        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        public SexyContent(int zoneId, int appId, bool enableCaching = true, int? ownerPortalId = null, ModuleInfo moduleInfo = null)
        {
            PortalSettingsOfOriginalModule = ownerPortalId.HasValue ? new PortalSettings(ownerPortalId.Value) : PortalSettings.Current;

            if (zoneId == 0)
                if (PortalSettingsOfOriginalModule == null || !ZoneHelpers.GetZoneID(PortalSettingsOfOriginalModule.PortalId).HasValue)
                    zoneId = Constants.DefaultZoneId;
                else
                    zoneId = ZoneHelpers.GetZoneID(PortalSettingsOfOriginalModule.PortalId).Value;

            if (appId == 0)
                appId = AppHelpers.GetDefaultAppId(zoneId);

            // Only disable caching of templates and contentgroupitems
            // if AppSetting "ToSIC_SexyContent_EnableCaching" is disabled
            if(enableCaching)
            {
                var cachingSetting = ConfigurationManager.AppSettings["ToSIC_SexyContent_EnableCaching"];
                if (!String.IsNullOrEmpty(cachingSetting) && cachingSetting.ToLower() == "false")
                    enableCaching = false;
            }

            Templates = new Templates(zoneId, appId);
			ContentGroups = new ContentGroups(zoneId, appId);

            // Set Properties on ContentContext
            ContentContext = EavDataController.Instance(zoneId, appId); // EavContext.Instance(zoneId, appId);
            ContentContext.UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? Settings.InternalUserName : HttpContext.Current.User.Identity.Name;

            ZoneId = zoneId;
            AppId = appId;


            #region Prepare Environment information 
            // 2016-01 2dm - this is new, the environment is where much code should go to later on

            // Build up the environment. If we know the module context, then use permissions from there
            Environment.Permissions = (moduleInfo != null)
                ? (IPermissions) new Environment.Dnn7.Permissions(moduleInfo)
                : new Environment.None.Permissions();
            #endregion
        }


        #endregion

        #region all dnn interface members moved to DnnBusinessController
        //     #region DNN Interface Members - search, upgrade

        //     /// <summary>
        //     /// Constructor overload for DotNetNuke
        //     /// (BusinessControllerClass needs parameterless constructor)
        //     /// </summary>
        //     public SexyContent() : this(0, 0) { }


        //     public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        //     {
        //         try
        //         {
        //             return new SearchController().GetModifiedSearchDocuments(moduleInfo, beginDate);
        //         }
        //         catch (Exception e)
        //         {
        //             throw new SearchIndexException(moduleInfo, e);
        //         }
        //     }

        //     /// <summary>
        //     /// This is part of the IUpgradeable of DNN
        //     /// </summary>
        //     /// <param name="version"></param>
        //     /// <returns></returns>
        //     public string UpgradeModule(string version)
        //     {
        //return SexyContentModuleUpgrade.UpgradeModule(version);
        //     }

        //     #endregion
        #endregion


        // --------------------------------------------------------------------------------------
        // 2016-02-27 2dm: the following stuff is all turned off, but still here in case errors appear and we need to see what was originally here
        #region removed / deprecated stuff, probably not needed any more

        //public const string ContentGroupItemIDString = "ContentGroupItemID";
        //public const string SortOrderString = "SortOrder";
        //public const string EntityID = "EntityID";
        //public const string AttributeSetIDString = "AttributeSetID";
        //public const string LocationIDCurrentPortal = "Portal File System";


        // 2016-02-26 2dm probably not needed any more
        //public static class ControlKeys
        //{
        //    public const string View = "";
        //    public const string Settings = "settings";
        //    public const string SettingsWrapper = "settingswrapper";
        //    public const string EavManagement = "eavmanagement";
        //    public const string EditContentGroup = "editcontentgroup";
        //    public const string EditTemplate = "edittemplate";
        //    public const string ManageTemplates = "managetemplates";
        //    public const string EditList = "editlist";
        //    public const string TemplateHelp = "templatehelp";
        //    public const string Export = "export";
        //    public const string Import = "import";
        //    public const string DataExport = "dataexport";
        //    public const string DataImport = "dataimport";
        //    public const string EditTemplateFile = "edittemplatefile";
        //    public const string EditTemplateDefaults = "edittemplatedefaults";
        //    public const string GettingStarted = "gettingstarted";
        //    public const string PortalConfiguration = "portalconfiguration";
        //    public const string EditDataSource = "editdatasource";
        //    public const string AppExport = "appexport";
        //    public const string AppImport = "appimport";
        //    public const string AppConfig = "appconfig";
        //    public const string PipelineManagement = "pipelinemanagement";
        //    public const string PipelineDesigner = "pipelinedesigner";
        //    public const string WebApiHelp = "WebApiHelp";
        //    public const string Permissions = "Permissions";
        //}


        #region URL Handling / Toolbar

        ///// <summary>
        ///// Get the URL for editing MetaData
        ///// </summary>
        ///// <param name="tabId"></param>
        ///// <param name="moduleId"></param>
        ///// <param name="returnUrl"></param>
        ///// <param name="portalSettings"></param>
        ///// <param name="control"></param>
        ///// <param name="attributeSetStaticName"></param>
        ///// <param name="assignmentObjectTypeID"></param>
        ///// <param name="keyNumber"></param>
        ///// <returns></returns>
        // 2016-02-26 2dm, probably not needed any more
        //public string GetMetaDataEditUrl(int tabId, int moduleId, string returnUrl, Control control, string attributeSetStaticName, int assignmentObjectTypeID, int keyNumber)
        //{
        //    var assignedEntity = DataSource.GetMetaDataSource(ZoneId.Value, AppId.Value).GetAssignedEntities(assignmentObjectTypeID, keyNumber, attributeSetStaticName).FirstOrDefault();
        //    var entityId = assignedEntity == null ? new int?() : assignedEntity.EntityId;

        //    return GetEntityEditLink(entityId , moduleId, tabId, attributeSetStaticName, returnUrl,
        //            assignmentObjectTypeID, keyNumber);
        //}

        // 2016-02-26 2dm, probably not needed any more
        //private string GetEntityEditLink(int? entityId, int moduleId, int tabId, string attributeSetStaticName, string returnUrl, int? assignmentObjectTypeId, int? keyNumber)
        //{
        //    var editUrl = Globals.NavigateURL(tabId, ControlKeys.EditContentGroup, new[] { "mid", moduleId.ToString(), "AppId", AppId.ToString(),
        //        "AttributeSetName", attributeSetStaticName, "AssignmentObjectTypeId", assignmentObjectTypeId.ToString(), "KeyNumber", keyNumber.ToString() });
        //    editUrl += (editUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);

        //    if (!entityId.HasValue)
        //        editUrl += "&EditMode=New";
        //    else
        //        editUrl += "&EntityId=" + entityId.Value;

        //    // If Culture exists, add CultureDimension
        //    var languageId = GetCurrentLanguageID();
        //    if (languageId.HasValue)
        //        editUrl += "&CultureDimension=" + languageId;

        //    return editUrl;
        //}

        ///// <summary>
        ///// Returns the Edit Link for given GroupID and SortOrder
        ///// </summary>
        ///// <param name="Element"></param>
        ///// <param name="ModuleID"></param>
        ///// <param name="TabID"></param>
        ///// <param name="UserID"></param>
        ///// <param name="ReturnUrl"></param>
        ///// <returns></returns>
        // 2016-02-26 2dm, probably not needed any more
        //public string GetElementEditLink(Guid ContentGroupID, int SortOrder, int ModuleID, int TabID, string ReturnUrl)
        //{
        //    var EditUrl = Globals.NavigateURL(TabID, ControlKeys.EditContentGroup, "mid", ModuleID.ToString(), SortOrderString, SortOrder.ToString(), "ContentGroupID", ContentGroupID.ToString());
        //    EditUrl += (EditUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);

        //    // If Culture exists, add CultureDimension
        //    var LanguageID = GetCurrentLanguageID();
        //    if(LanguageID.HasValue)
        //        EditUrl += "&CultureDimension=" + LanguageID;

        //    return EditUrl;
        //}

        // 2016-02-26 2dm, probably not needed any more
        //public string GetElementAddWithEditLink(Guid ContentGroupID, int DestinationSortOrder, int ModuleID, int TabID, string ReturnUrl)
        //{
        //    return GetElementEditLink(ContentGroupID, DestinationSortOrder, ModuleID, TabID, ReturnUrl) + "&EditMode=New";
        //}

        // 2016-02-26 2dm, probably not needed any more
        //public string GetElementSettingsLink(Guid ContentGroupID, int sortOrder, int ModuleID, int TabID, string ReturnUrl)
        //{
        //    var settingsUrl = Globals.NavigateURL(TabID, ControlKeys.SettingsWrapper, "mid", ModuleID.ToString(), ContentGroupGuidString, ContentGroupID.ToString(), "SortOrder", sortOrder.ToString(), "ItemType", sortOrder == -1 ? "ListContent" : "Content");
        //    settingsUrl += (settingsUrl.IndexOf("?") == -1 ? "?" : "&") + "popUp=true&ReturnUrl=" + HttpUtility.UrlEncode(ReturnUrl);
        //    return settingsUrl;
        //}

        #endregion


        // 2016-02-26 2dm probably unused
        //public bool IsEditMode()
        //{
        //    return Globals.IsEditMode() && (OwnerPS.PortalId == PS.PortalId);
        //}


        // 2016-02-26 2dm - was only used once, moved it into that metho
        ///// <summary>
        ///// Configures a Portal (Creates a 2sxc Folder containing a web.config File)
        ///// </summary>
        //public void ConfigurePortal(HttpServerUtility server)
        //{
        //    var tm = new TemplateManager(this);
        //    tm.EnsureTemplateFolderExists(server, TemplateLocations.PortalFileSystem);
        //}

        // 2016-02-26 2dm probably not used any more
        ///// <summary>
        ///// Gets the initial DataSource
        ///// </summary>
        ///// <param name="zoneId"></param>
        ///// <param name="appId"></param>
        ///// <returns></returns>
        //public static IDataSource GetInitialDataSource(int zoneId, int appId, bool showDrafts = false)
        //{
        //    return DataSource.GetInitialDataSource(zoneId, appId, showDrafts);
        //}



        // 2016-02-26 2dm - moved this out into the dynamic-object and renamed from previously GetDictionaryFromEntity
        //    internal Dictionary<string, object> ToDictionary(IEntity entity, string language)
        //    {
        //        var dynamicEntity = new DynamicEntity(entity, new[] { language }, this);
        //        bool propertyNotFound;

        //        // Convert DynamicEntity to dictionary
        //        var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
        //        {
        //            var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
        //            if (v.Type == "Entity" && value is List<DynamicEntity>)
        //                return ((List<DynamicEntity>) value).Select(p => new { p.EntityId, p.EntityTitle });
        //            return value;
        //        }));

        //        dictionary.Add("EntityId", entity.EntityId);
        //        dictionary.Add("Modified", entity.Modified);

        //     if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
        //     {
        //      var entityInGroup = (EntityInContentGroup) entity;
        //if(entityInGroup.Presentation != null)
        //	dictionary.Add("Presentation", ToDictionary(entityInGroup.Presentation, language));
        //     }

        //     if(entity is IHasEditingData)
        //            dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
        //        else
        //            dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[language] : "(no title)" });

        //        return dictionary;
        //    }



        //     /// <summary>
        //     /// Resolves File and Page values
        //     /// For example, File:123 or Page:123
        //     /// </summary>
        //     public static string ResolveHyperlinkValues(string value, PortalSettings ownerPortalSettings)
        //     {
        //         var resultString = value;
        //         var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

        //         if (!regularExpression.Success)
        //             return value;

        //         var fileManager = FileManager.Instance;
        //         var tabController = new TabController();
        //         var type = regularExpression.Groups["type"].Value.ToLower();
        //         var id = int.Parse(regularExpression.Groups["id"].Value);
        //var urlParams = regularExpression.Groups["params"].Value ?? "";


        //         switch (type)
        //         {
        //             case "file":
        //                 var fileInfo = fileManager.GetFile(id);
        //                 if (fileInfo != null)
        //                     resultString = (fileInfo.StorageLocation == (int)FolderController.StorageLocationTypes.InsecureFileSystem
        //                         ? Path.Combine(ownerPortalSettings.HomeDirectory, fileInfo.RelativePath) + urlParams
        //                         : fileManager.GetUrl(fileInfo));
        //                 break;
        //             case "page":
        //                 var portalSettings = PortalSettings.Current;

        //                 // Get full PortalSettings (with portal alias) if module sharing is active
        //                 if (PortalSettings.Current != null && PortalSettings.Current.PortalId != ownerPortalSettings.PortalId)
        //                 {
        //                     var portalAlias = ownerPortalSettings.PrimaryAlias ?? TestablePortalAliasController.Instance.GetPortalAliasesByPortalId(ownerPortalSettings.PortalId).First();
        //                     portalSettings = new PortalSettings(id, portalAlias);
        //                 }
        //                 var tabInfo = tabController.GetTab(id, ownerPortalSettings.PortalId, false);
        //                 if (tabInfo != null)
        //                 {
        //                     if (tabInfo.CultureCode != "" && tabInfo.CultureCode != PortalSettings.Current.CultureCode)
        //                     {
        //                         var cultureTabInfo = tabController.GetTabByCulture(tabInfo.TabID, tabInfo.PortalID, LocaleController.Instance.GetLocale(PortalSettings.Current.CultureCode));

        //                         if (cultureTabInfo != null)
        //                             tabInfo = cultureTabInfo;
        //                     }

        //                     // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
        //                     resultString = Globals.NavigateURL(tabInfo.TabID, portalSettings, "", new string[] { }) + urlParams;
        //                 }
        //                 break;
        //         }

        //         return resultString;
        //     }

        // 2016-02-27 2dm - doesn't seem to be used any where
        //public static void AddDNNVersionToBodyClass(Control Parent)
        //{
        //    // Add DNN Version to body as CSS Class
        //    var CssClass = "dnn-" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.Major;
        //    var body = (HtmlGenericControl)Parent.Page.FindControl("ctl00$body");
        //    if(body.Attributes["class"] != null)
        //        body.Attributes["class"] += CssClass;
        //    else
        //        body.Attributes["class"] = CssClass;
        //}


        // 2016-02-27 2dm - seems unused
        //public string GetCurrentLanguageName()
        //{
        //    return Thread.CurrentThread.CurrentCulture.Name;
        //}


        // 2016-02-27 2dm - seems unused
        //public static int? GetLanguageId(int zoneId, string externalKey)
        //{
        //    return new SexyContent(zoneId, AppHelpers.GetDefaultAppId(zoneId)).ContentContext.Dimensions.GetLanguageId(externalKey);
        //}


        //#region Culture Handling

        // 2016-02-27 2dm - seems unused, was only used in the SexyViewContentOrApp to pass to JS, but it seems it was never used after that
        //[Obsolete("Don't use this anymore, use 'GetCurrentLanguageName' (work with strings)")]
        //public int? GetCurrentLanguageID(bool UseDefaultLanguageIfNotFound = false)
        //{
        //    var LanguageID = ContentContext.Dimensions.GetLanguageId(Thread.CurrentThread.CurrentCulture.Name);
        //    if (!LanguageID.HasValue && UseDefaultLanguageIfNotFound)
        //        LanguageID = ContentContext.Dimensions.GetLanguageId(PortalSettings.Current.DefaultLanguage);
        //    return LanguageID;
        //}

        // 2016-02-27 2dm - moved into EAV - Dimensions object
        //public void AddOrUpdateLanguage(string cultureCode, string cultureText, bool Active, int PortalID)
        //{
        //    var EAVLanguage = ContentContext.Dimensions.GetLanguages().Where(l => l.ExternalKey == cultureCode).FirstOrDefault();
        //    // If the language exists in EAV, set the active state, else add it
        //    if (EAVLanguage != null)
        //        ContentContext.Dimensions.UpdateDimension(EAVLanguage.DimensionID, Active);
        //    else
        //    {
        //        //var cultureText = LocaleController.Instance.GetLocale(cultureCode).Text;
        //        ContentContext.Dimensions.AddLanguage(cultureText, cultureCode);
        //    }
        //}


        // #endregion

        // #region Ensure Portal is configured when working with 2sxc

        // 2016-02-27 2dm - moved out...
        ///// <summary>
        ///// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        ///// </summary>
        //public void EnsurePortalIsConfigured(HttpServerUtility server, string controlPath)
        //{
        //    var sexyFolder = new DirectoryInfo(server.MapPath(Path.Combine(OwnerPS.HomeDirectory, SexyContent.TemplateFolder)));
        //    var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, "Content"));
        //    var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, SexyContent.WebConfigFileName));
        //    if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
        //    {
        //        // configure it
        //        var tm = new TemplateManager(this);
        //        tm.EnsureTemplateFolderExists(server, SexyContent.TemplateLocations.PortalFileSystem);
        //    };
        //}


        //#endregion


        #region Get DataSources

        // 2015-04-30 2dm must cache this, shouldn't get re-created on a single call, it's always the same
        // todo: must ask 2rm if it's ok to cache - can this SexyContent be re-used for other modules?
        //   private ValueCollectionProvider _valueCollectionProvider;
        //public ValueCollectionProvider GetConfigProviderForModule(int moduleId, PortalSettings portalSettings)
        //   {
        //    if (_valueCollectionProvider != null) return _valueCollectionProvider;

        //    var provider = new ValueCollectionProvider();

        //    // only add these in running inside an http-context. Otherwise leave them away!
        //    if (HttpContext.Current != null)
        //    {
        //        var request = HttpContext.Current.Request;
        //        provider.Sources.Add("querystring", new FilteredNameValueCollectionPropertyAccess("querystring", request.QueryString));
        //        provider.Sources.Add("server", new FilteredNameValueCollectionPropertyAccess("server", request.ServerVariables));
        //        provider.Sources.Add("form", new FilteredNameValueCollectionPropertyAccess("form", request.Form));
        //    }

        //    // Add the standard DNN property sources if PortalSettings object is available
        //    if (portalSettings != null)
        //    {
        //        var dnnUsr = portalSettings.UserInfo;
        //        var dnnCult = Thread.CurrentThread.CurrentCulture;
        //        var dnn = new TokenReplaceDnn(App, moduleId, portalSettings, dnnUsr);
        //        var stdSources = dnn.PropertySources;
        //        foreach (var propertyAccess in stdSources)
        //            provider.Sources.Add(propertyAccess.Key,
        //                new ValueProviderWrapperForPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));
        //    }

        //    provider.Sources.Add("app", new AppPropertyAccess("app", App));

        //    // add module if it was not already added previously
        //    if (!provider.Sources.ContainsKey("module"))
        //    {
        //        var modulePropertyAccess = new StaticValueProvider("module");
        //        modulePropertyAccess.Properties.Add("ModuleID", moduleId.ToString(CultureInfo.InvariantCulture));
        //        provider.Sources.Add(modulePropertyAccess.Name, modulePropertyAccess);
        //    }
        //    _valueCollectionProvider = provider;
        //    return _valueCollectionProvider;
        //   }

        ///// <summary>
        ///// The EAV DataSource
        ///// </summary>
        //private IDataSource _viewDataSource;// { get; set; }

        //public IDataSource GetViewDataSource(int moduleId, bool showDrafts, Template template)
        //{
        //    if (_viewDataSource != null) return _viewDataSource;
        //    _viewDataSource = ViewDataSource.ForModule(moduleId, showDrafts, template, this);
        //    return _viewDataSource;

        //2016-02-27 2dm - extracted this away from here
        //var configurationProvider = GetConfigProviderForModule(moduleId, PS);

        //// Get ModuleDataSource
        //var initialSource = DataSource.GetInitialDataSource(ZoneId, AppId, showDrafts);
        //var moduleDataSource = DataSource.GetDataSource<ModuleDataSource>(ZoneId, AppId, initialSource, configurationProvider);
        //moduleDataSource.ModuleId = moduleId;
        //if(template != null)
        //    moduleDataSource.OverrideTemplateId = template.TemplateId;
        //moduleDataSource.Sexy = this;

        //var viewDataSourceUpstream = moduleDataSource;

        //// If the Template has a Data-Pipeline, use it instead of the ModuleDataSource created above
        //if (template != null && template.Pipeline != null)
        //    viewDataSourceUpstream = null;

        //var viewDataSource = DataSource.GetDataSource<ViewDataSource>(ZoneId, AppId, viewDataSourceUpstream, configurationProvider);

        //// Take Publish-Properties from the View-Template
        //if (template != null)
        //{
        //    viewDataSource.Publish.Enabled = template.PublishData;
        //    viewDataSource.Publish.Streams = template.StreamsToPublish;

        //    // Append Streams of the Data-Pipeline (this doesn't require a change of the viewDataSource itself)
        //    if (template.Pipeline != null)
        //        DataPipelineFactory.GetDataSource(AppId.Value, template.Pipeline.EntityId, configurationProvider, viewDataSource);
        //}

        //_viewDataSource = viewDataSource;

        //return _viewDataSource;
        //}

        // 2016-02-27 2dm - removed, it was only used in 
        ///// <summary>
        ///// This is the PS of the current page.
        ///// </summary>
        //internal PortalSettings PortalSettingsOfVisitedPage => PortalSettings.Current;

        ///// <summary>
        ///// This is needed so when the application starts, we can configure our IoC container
        ///// </summary>
        //static SexyContent()
        //{
        //    new UnityConfig().Configure();
        //    SetEavConnectionString();
        //}

        // 2016-02-27 2dm - moved to settings
        ///// <summary>
        ///// Set EAV's connection string to DNN's
        ///// </summary>
        //public static void SetEavConnectionString()
        //{
        //    Eav.Configuration.SetConnectionString("SiteSqlServer");
        //}

        #endregion
        #endregion
    }
}