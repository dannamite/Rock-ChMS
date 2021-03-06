﻿//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Collections.Generic;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace RockWeb.Blocks.Finance
{
    /// <summary>
    /// 
    /// </summary>
    // TODO: Create a fund list field attribute
    [CustomDropdownListField( "Fund", "The fund that new pledges will be allocated toward.",
        listSource: "SELECT [Id] AS 'Value', [PublicName] AS 'Text' FROM [Fund] WHERE [IsPledgable] = 1 ORDER BY [Order]",
        key: "DefaultFund", required: true, order: 0 )]
    [TextField( "Legend Text", "Custom heading at the top of the form.", key: "LegendText", defaultValue: "Create a new pledge", order: 1 )]
    [LinkedPage( "Giving Page", "The page used to set up a person's giving profile.", key: "GivingPage", order: 2 )]
    [DateField( "Start Date", "Date all pledges will begin on.", key: "DefaultStartDate", order: 3 )]
    [DateField( "End Date", "Date all pledges will end.", key: "DefaultEndDate", order: 4 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_STATUS, "New User Status", "Person status to assign to a new user.", key: "DefaultPersonStatus", order: 5 )]
    public partial class CreatePledge : RockBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !IsPostBack )
            {
                Session.Remove( "CachedPledge" );
                lLegendText.Text = GetAttributeValue( "LegendText" );
                ShowForm();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            using ( new UnitOfWorkScope() )
            {
                RockTransactionScope.WrapTransaction( () =>
                    {
                        var pledgeService = new PledgeService();
                        var defaultFundId = int.Parse( GetAttributeValue( "DefaultFund" ) );
                        var person = FindPerson();
                        var pledge = FindAndUpdatePledge( person, defaultFundId );

                        // Does this person already have a pledge for this fund?
                        // If so, give them the option to create a new one?
                        if ( person.Pledges.Any( p => p.FundId == defaultFundId ) )
                        {
                            pnlConfirm.Visible = true;
                            Session.Add( "CachedPledge", pledge );
                            return;
                        }

                        if ( pledge.IsValid )
                        {
                            pledgeService.Add( pledge, person.Id );
                            pledgeService.Save( pledge, person.Id );
                            // TODO: Queue up email copy of receipt to send to user
                            ShowReceipt();
                        }
                    });
            }
        }

        protected void btnConfirmYes_Click( object sender, EventArgs e )
        {
            var pledge = Session["CachedPledge"] as Pledge;
            
            if ( pledge != null && pledge.IsValid )
            {
                var pledgeService = new PledgeService();
                pledgeService.Add( pledge, CurrentPersonId );
                pledgeService.Save( pledge, CurrentPersonId );
                Session.Remove( "CachedPledge" );
                ShowReceipt();
            }
        }

        protected void btnConfirmNo_Click( object sender, EventArgs e )
        {
            pnlConfirm.Visible = false;
        }

        /// <summary>
        /// Shows the view.
        /// </summary>
        private void ShowForm()
        {
            var frequencyTypeGuid = new Guid( Rock.SystemGuid.DefinedType.FINANCIAL_PLEDGE_FREQUENCY_TYPE );
            ddlFrequencyType.BindToDefinedType( DefinedTypeCache.Read( frequencyTypeGuid ) );

            if ( CurrentPerson != null )
            {
                tbFirstName.Text = !string.IsNullOrWhiteSpace( CurrentPerson.NickName ) ? CurrentPerson.NickName : CurrentPerson.FirstName;
                tbLastName.Text = CurrentPerson.LastName;
                tbEmail.Text = CurrentPerson.Email;
            }

            var start = GetAttributeValue( "DefaultStartDate" );
            var end = GetAttributeValue( "DefaultEndDate" );

            if ( string.IsNullOrWhiteSpace( start ) )
            {
                dtpStartDate.Visible = true;
            }

            if ( string.IsNullOrWhiteSpace( end ) )
            {
                dtpEndDate.Visible = true;
            }

            var pledge = Session["CachedPledge"] as Pledge;

            if ( pledge != null )
            {
                pnlConfirm.Visible = true;
            }
        }

        private void ShowReceipt()
        {
            pnlForm.Visible = false;
            pnlReceipt.Visible = true;
            btnGivingProfile.NavigateUrl = string.Format( "~/Page/{0}", GetAttributeValue( "GivingPage" ) );
        }

        /// <summary>
        /// Finds the person if they're logged in, or by email and name. If not found, creates a new person.
        /// </summary>
        /// <returns></returns>
        private Person FindPerson()
        {
            Person person;
            var personService = new PersonService();

            if ( CurrentPerson != null )
            {
                person = CurrentPerson;
            }
            else
            {
                person = personService.GetByEmail( tbEmail.Text )
                    .FirstOrDefault( p => p.FirstName == tbFirstName.Text && p.LastName == tbLastName.Text );
            }

            if ( person == null )
            {
                var definedValue = new DefinedValueService().Get( new Guid( GetAttributeValue( "DefaultPersonStatus" ) ) );
                person = new Person
                {
                    GivenName = tbFirstName.Text,
                    LastName = tbLastName.Text,
                    Email = tbEmail.Text,
                    PersonStatusValueId = definedValue.Id,
                    Pledges = new List<Pledge>()
                };

                personService.Add( person, CurrentPersonId );
                personService.Save( person, CurrentPersonId );
            }

            return person;
        }

        /// <summary>
        /// Finds the pledge.
        /// </summary>
        /// <param name="person">The Person.</param>
        /// <param name="fundId">The Fund Id</param>
        /// <returns></returns>
        private Pledge FindAndUpdatePledge( Person person, int fundId )
        {
            var pledge = Session["CachedPledge"] as Pledge;

            if ( pledge == null )
            {
                pledge = new Pledge();
            }

            pledge.PersonId = person.Id;
            pledge.FundId = fundId;
            pledge.Amount = decimal.Parse( tbAmount.Text );

            var startSetting = GetAttributeValue( "DefaultStartDate" );
            var startDate = !string.IsNullOrWhiteSpace( startSetting ) ? DateTime.Parse( startSetting ) : DateTime.Parse( dtpStartDate.Text );
            var endSetting = GetAttributeValue( "DefaultEndDate" );
            var endDate = !string.IsNullOrWhiteSpace( endSetting ) ? DateTime.Parse( endSetting ) : DateTime.Parse( dtpEndDate.Text );
            pledge.StartDate = startDate;
            pledge.EndDate = endDate;

            pledge.FrequencyTypeValueId = int.Parse( ddlFrequencyType.SelectedValue );
            pledge.FrequencyAmount = decimal.Parse( tbFrequencyAmount.Text );
            return pledge;
        }
    }
}
