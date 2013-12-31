# Rhythm Umbraco Extensions

## Constants

##### Document Types
Constants for common Umbraco Document Types.

##### Properties
Constants for common Umbraco Properties.


## Contour Prevalue Sources

##### PrevaluesFromNodesOfType
Contour prevalue source type that gets content nodes of a specified type.

## Contour Workflows

##### SubscribeToExactTargetNewsletter
Workflow type that subscribes the user to a newsletter using the Exact Target API.


## Data Types

##### Access Code
Simple Data Type for generating random access codes. This is a read-only Data Type.

##### Font Awesome Icon Picker
Displays a drop-down that lists all of the FontAwesome icons in it along with the icon itself.


## Extension Methods

##### DateTimeExtensionMethods

string ToGmt(this DateTime date)

	Converts a DateTime instance to a GMT string.

int TotalMonths(this DateTime start, DateTime end)

	Returns the number of months between two dates.


##### ExpandoObjectExtensionMethods

bool HasProperty(this ExpandoObject item, string propertyName)

	Checks if a property by the specified name exists on the specified object.


##### FormExtensionMethods

Form GetFormFromName(this FormStorage formStorage, string name)
	
	Gets an Umbraco Contour form based on it's name.

Guid GetFormIdFromName(this FormStorage formStorage, string name)
	Gets an Umbraco Contour form ID based on it's name.

##### HtmlHelperExtensionMethods

string GetCssClass(this HtmlHelper helper, string str)

	Converts a string to a CSS class (e.g., "hello, World" would be come "hello-world").

string NormalizeUrl(this System.Web.Mvc.HtmlHelper helper, string url)

	Ensures the specified URL starts with "http://".

##### ObjectExtensionMethods

ExpandoObject ToExpando(this object source)

	Makes the specified object an expando object.

##### PublishedContentExtensionMethods

string GetTitle(this IPublishedContent content)

	Returns the "title" property. If the "title" property does not exist it uses the node's name instead.

string ToJson(this IPublishedContent content)
	
	Converts a content node into a JSON string.

string ToJson(this IEnumerable<IPublishedContent> content)

	Converts a collection of content nodes into a JSON string.

JObject ToJObject(this IPublishedContent content)

	Converts a content node into a JObject instance.

JArray ToJArray(this IEnumerable<IPublishedContent> content)

	Converts a collection of content nodes into a JArray instance.
	
string LocalizedPropertyValue(this IPublishedContent source, string propertyAlias)
	
	Returns the localized value of the specified property.
	
T LocalizedPropertyValue<T>(this IPublishedContent source, string propertyAlias)

	Returns the localized value of the specified property.
	
IPublishedContent LocalizedGetPickedNode(this IPublishedContent source, string propertyAlias, bool recursive = false)

	Gets the picked node.
	
IEnumerable<IPublishedContent> LocalizedGetPickedNodes(this IPublishedContent source, string propertyAlias, bool recursive = false)

	Gets the picked nodes.
	
IPublishedContent LocalizedGetPickedMedia(this IPublishedContent source, string propertyAlias, bool recursive = false)

	Gets the picked media.
	
T LocalizedGetSetting<T>(this IPublishedContent source, string settingKey)

	Gets a setting value.

string LocalizedDropDownValue(this IPublishedContent source, string propertyAlias, bool recursive = false)

	Gets a drop down value (aka, a pre-value) as a string.
	
IPublishedContent NearestAncestorOfType(this IPublishedContent source, string typeAlias, bool includeSelf = false)

	Searches for the nearest ancestor with the specified content type.
	
T LocalizedPropertyValueHelper<T>(DynamicNode page, string propertyAlias)

	Returns the localized value of the specified property.

T GetPropertyValue<T>(int nodeId, string propertyAlias)

	Gets the typed value of the property on the specified node ID.
	
string LocalizationSelectedLanguage()

	Returns user's selected language.
	
UmbracoHelper GetHelper()
	
	Gets an Umbraco helper.
	

##### StringExtensionMethods

string ToMd5Hash(this string input)
	
	Converts a string into an MD5 hash.	

##### UmbracoHelperExtensionMethods

IPublishedContent GetContentAtRootByDocumentType(this UmbracoHelper helper, string documentType)
	
	Gets the root most content node of a specific document type.

IPublishedContent GetHome(this UmbracoHelper helper)

	Gets the root home node.

IEnumerable<IPublishedContent> TypedSearchInHome(this UmbracoHelper helper, string query, int take = 999, int skip = 0)

	Searches within the home node for content nodes that match the query.

##### XmlExtensionMethods

string ToJson(this XmlDocument document)

	Converts an XmlDocument into a JSON string.

JObject ToJObject(this XmlDocument document)

	Converts an XmlDocument into a JObject.

bool TryParseXml(this XmlDocument document, string xml)

	Tries to parse a string into an XmlDocument and returns whether the operation was successful.

bool TryParseXml(string xml)

	Tries to parse a string into an XmlDocument and returns whether the operation was successful.

XmlDocument LoadXml(string xml)

	Loads an xml string into an XmlDocument and returns the XmlDocument instance.
	
	
	
## Models

##### SimpleContent
Class that represents the bare minimum data for a page to serialize to JSON

##### SimpleContentType
Class that represents the bare minimum data for a content type to serialize to JSON

##### SimpleTemplate
Class that represents the bare minimum data for a page to serialize to JSON


## Utilities

##### FontAwesomeCssParser
Service responsible for applying the regex and retrieving the class names

##### NetUtility
Utility to help with network operations.

##### NotNull
Wraps an instance to ensure it can be passed around with a non-null instance.

##### PasswordGenerator
Simple password generator class.

##### PrevalueUtility
Utility to help work with Umbraco prevalues.

##### ReflectionUtility
Utility to help with reflection operations.
