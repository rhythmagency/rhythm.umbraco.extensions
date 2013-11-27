namespace Rhythm.Extensions.Contour.PrevalueSourceTypes
{

    // Namespaces.
    using Rhythm.Extensions.ExtensionMethods;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Forms.Core;
    using IContent = Umbraco.Core.Models.IContent;
    using Setting = Umbraco.Forms.Core.Attributes.Setting;
    using UmbracoContext = Umbraco.Web.UmbracoContext;
    using UmbracoHelper = Umbraco.Web.UmbracoHelper;


    /// <summary>
    /// Contour prevalue source type that gets content nodes of a specified type.
    /// </summary>
    public class PrevaluesFromNodesOfType : FieldPreValueSourceType
    {

        #region Properties

        /// <summary>
        /// The primary content type (also serves as a reference list to view all content types).
        /// </summary>
        [Setting("Primary Content Type", description = "Required. The primary content type.", control = "Umbraco.Forms.Core.FieldSetting.Pickers.DocumentType")]
        public string PrimaryContentType { get; set; }


        /// <summary>
        /// The additional content types (since the document type picker only allows one to be chosen).
        /// </summary>
        [Setting("Additional Content Types", description = "Optional. The additional content types (comma-separated aliases).", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
        public string AdditionalContentTypes { get; set; }


        /// <summary>
        /// The primary property to label the prevalue with.
        /// </summary>
        [Setting("Primary Property", description = "Required. The primary property to display (alias, * for node name).", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
        public string PrimaryProperty { get; set; }


        /// <summary>
        /// The secondary properties to label the prevalue with (the first one found will be used).
        /// </summary>
        [Setting("Secondary Properties", description = "Optional. The secondary property to display (comma-separated aliases, * for node name).", control = "Umbraco.Forms.Core.FieldSetting.TextField")]
        public string SecondaryProperty { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PrevaluesFromNodesOfType()
        {
            this.Id = new Guid("2F768E43-6796-4A6D-93EE-7EB9B2B45FC5");
            this.Name = "Get Prevalues from Content Tree";
            this.Description = "Gets prevalues based on content nodes of the specified types.";
        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets the prevalues.
        /// </summary>
        public override List<PreValue> GetPreValues(Field field)
        {

            // Variables.
            var prevalues = new List<PreValue>();


            // Only proceed if a content type has been chosen.
            if (!string.IsNullOrWhiteSpace(PrimaryContentType))
            {

                // Variables.
                var helper = new UmbracoHelper(UmbracoContext.Current);
                var typeService = Umbraco.Core.ApplicationContext.Current.Services.ContentTypeService;
                var contentService = Umbraco.Core.ApplicationContext.Current.Services.ContentService;
                var separators = new string[] { ", ", ",", " " };
                var removeEmpties = StringSplitOptions.RemoveEmptyEntries;
                var splitTypes = (AdditionalContentTypes ?? string.Empty).Split(separators, removeEmpties);
                var secondaryAliases = (SecondaryProperty ?? string.Empty).Split(separators, removeEmpties);
                var typeIds = new List<int>();


                // Get content type ID's.
                typeIds.Add(int.Parse(PrimaryContentType));
                foreach (var typeAlis in splitTypes)
                {
                    var extraType = typeService.GetContentType(typeAlis);
                    if (extraType != null && extraType.Id > 0)
                    {
                        typeIds.Add(extraType.Id);
                    }
                }
                typeIds = typeIds.Distinct().ToList();


                // Get published content nodes of specified types.
                var nodes = new List<IContent>();
                foreach (var typeId in typeIds)
                {
                    nodes.AddRange(contentService.GetContentOfContentType(typeId));
                }
                var nodeIds = nodes.Select(x => x.Id).Distinct().ToArray();
                var publishedNodes = helper.TypedContent(nodeIds);


                // Get prevalues.
                foreach (var node in publishedNodes)
                {

                    // Variables.
                    string prop1 = null;
                    string prop2 = null;


                    // First property value.
                    if (PrimaryProperty == "*")
                    {
                        prop1 = node.Name;
                    }
                    else
                    {
                        prop1 = node.LocalizedPropertyValue<string>(PrimaryProperty);
                    }


                    // Second property value.
                    foreach (var alias in secondaryAliases)
                    {
                        if (alias == "*")
                        {
                            prop2 = node.Name;
                        }
                        else
                        {
                            prop2 = node.LocalizedPropertyValue<string>(alias);
                        }
                        if (!string.IsNullOrWhiteSpace(prop2))
                        {
                            break;
                        }
                    }


                    // Remove leading property values.
                    if (string.IsNullOrWhiteSpace(prop1))
                    {
                        prop1 = prop2;
                        prop2 = null;
                    }


                    // Combine property values.
                    string valueName = prop1 + (string.IsNullOrWhiteSpace(prop2)
                        ? string.Empty
                        : string.Format(" ({0})", prop2));
                    if (string.IsNullOrWhiteSpace(valueName))
                    {
                        valueName = node.Name;
                    }


                    // Create prevalue.
                    if (!string.IsNullOrWhiteSpace(valueName))
                    {
                        prevalues.Add(new PreValue() { Id = node.Id, Value = valueName });
                    }

                }

            }


            // Sort and finalize prevalues.
            prevalues = prevalues.OrderBy(x => x.Value).ToList();
            int sort = 0;
            foreach (var prevalue in prevalues)
            {
                if (field != null)
                {
                    prevalue.Field = field.Id;
                }
                prevalue.SortOrder = sort;
                sort++;
            }


            // Return prevalues.
            return prevalues;

        }


        /// <summary>
        /// Ensures the settings are valid.
        /// </summary>
        public override List<Exception> ValidateSettings()
        {
            var errors = new List<Exception>();
            if (string.IsNullOrWhiteSpace(PrimaryContentType))
            {
                errors.Add(new Exception("Primary Content Type is required."));
            }
            if (string.IsNullOrWhiteSpace(PrimaryProperty))
            {
                errors.Add(new Exception("Primary Property is required."));
            }
            return errors;
        }

        #endregion

    }

}