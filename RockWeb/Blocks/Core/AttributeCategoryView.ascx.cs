﻿//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock;
using Rock.Core;
using Rock.Field;
using Rock.Web.UI.Controls;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace RockWeb.Blocks.Core
{
    /// <summary>
    /// User control for editing the value(s) of a set of attributes for a given entity and category
    /// </summary>
    [Rock.Attribute.Property( 1, "Entity Qualifier Column", "Filter", "The entity column to evaluate when determining if this attribute applies to the entity", false, "" )]
    [Rock.Attribute.Property( 2, "Entity Qualifier Value", "Filter", "The entity column value to evaluate.  Attributes will only apply to entities with this value", false, "" )]
    [Rock.Attribute.Property( 3, "Attribute Categories", "Filter", "Delimited List of Attribute Category Names", true, "" )]
	[Rock.Attribute.Property( 4, "Xslt File", "Behavior", "XSLT File to use.", false, "AttributeValues.xslt" )]
	public partial class AttributeCategoryView : Rock.Web.UI.ContextBlock
    {
		private XDocument xDocument = null;

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            
            string entityQualifierColumn = AttributeValue( "EntityQualifierColumn" );
            if ( string.IsNullOrWhiteSpace( entityQualifierColumn ) )
                entityQualifierColumn = PageParameter( "EntityQualifierColumn" );

            string entityQualifierValue = AttributeValue( "EntityQualifierValue" );
            if ( string.IsNullOrWhiteSpace( entityQualifierValue ) )
                entityQualifierValue = PageParameter( "EntityQualifierValue" );

            ObjectCache cache = MemoryCache.Default;
            string cacheKey = string.Format( "Attributes:{0}:{1}:{2}", base.EntityType, entityQualifierColumn, entityQualifierValue );

            Dictionary<string, List<int>> cachedAttributes = cache[cacheKey] as Dictionary<string, List<int>>;
            if ( cachedAttributes == null )
            {
                cachedAttributes = new Dictionary<string, List<int>>();

                AttributeService attributeService = new AttributeService();
                foreach ( var item in attributeService.Queryable().
                    Where( a => a.Entity == EntityType &&
                        ( a.EntityQualifierColumn ?? string.Empty ) == entityQualifierColumn &&
                        ( a.EntityQualifierValue ?? string.Empty ) == entityQualifierValue ).
                    OrderBy( a => a.Category ).
                    ThenBy( a => a.Order ).
                    Select( a => new { a.Category, a.Id } ) )
                {
                    if ( !cachedAttributes.ContainsKey( item.Category ) )
                        cachedAttributes.Add( item.Category, new List<int>() );
                    cachedAttributes[item.Category].Add( item.Id );
                }

                CacheItemPolicy cacheItemPolicy = null;
                cache.Set( cacheKey, cachedAttributes, cacheItemPolicy );
            }

            Rock.Attribute.IHasAttributes model = base.Entity as Rock.Attribute.IHasAttributes;
            if ( model != null )
            {
				var rootElement = new XElement( "root" );

				foreach ( string category in AttributeValue( "AttributeCategories" ).SplitDelimitedValues(false) )
				{
					if ( cachedAttributes.ContainsKey( category ) )
					{
						var attributesElement = new XElement( "attributes",
							new XAttribute( "category-name", category )
							);
						rootElement.Add( attributesElement );

						foreach ( var attributeId in cachedAttributes[category] )
						{
							var attribute = Rock.Web.Cache.AttributeCache.Read( attributeId );
							if ( attribute != null )
							{
								var values = model.AttributeValues[attribute.Key].Value;
								if ( values != null && values.Count > 0 )
								{
									attributesElement.Add( new XElement("attribute",
										new XAttribute("name", attribute.Name),
										new XCData(attribute.FieldType.Field.FormatValue( null, values[0].Value, attribute.QualifierValues, false ) ?? string.Empty )
									));
								}
							}
						}
					}
				}

				xDocument = new XDocument( new XDeclaration( "1.0", "UTF-8", "yes" ), rootElement );

            }
        }

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			try
			{
				if ( xDocument != null && !String.IsNullOrEmpty( AttributeValue( "XsltFile" ) ) )
				{
					string xsltFile = AttributeValue( "XsltFile" );
					if ( !String.IsNullOrEmpty( xsltFile ) )
					{
						string xsltPath = Server.MapPath( "~/Themes/" + CurrentPage.Site.Theme + "/Assets/Xslt/" + AttributeValue( "XsltFile" ) );
						var xslt = new XslCompiledTransform();
						xslt.Load( xsltPath );
						xslt.Transform( xDocument.CreateReader(), null, writer );
					}
				}
			}
			catch ( Exception ex )
			{
				writer.Write( "Error: " + ex.Message );
			}
		}

    }
}