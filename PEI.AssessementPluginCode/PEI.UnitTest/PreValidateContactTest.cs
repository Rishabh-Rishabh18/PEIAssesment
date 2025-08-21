using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PEI.Assesment.Plugins;
using Microsoft.Xrm.Sdk;

using System.Linq;
using FakeXrmEasy.Plugins;

namespace PEI.UnitTest
{
    /// <summary>
    /// Unit test cases for plugin PreValidateCContact.
    /// This class contains methods to test the PreValidateCContact plugin functionality.
    /// </summary>
    [TestClass]
    public class PreValidateContactTest
    {
        // intiating the fake xrm properties
        internal FakeXrm fakeXrm = new FakeXrm();
        
        /// <summary>
        /// Creating a contact with an existing email address.
        /// This test checks if the plugin throws an exception when trying to create a contact with an email address that already exists in the system.
        /// </summary>
        [TestMethod]
        public void expectPluginException()
        {
            //initialize
            fakeXrm.PluginExecutionContext.MessageName = "Create";// Create event
            fakeXrm.PluginExecutionContext.Stage = 10;// Pre validate

            // Initilazing the existing contact
            Entity existingContact = new Entity { Id = Guid.NewGuid(), LogicalName = "contact", ["emailaddress1"] = "test@test.com" };
            fakeXrm.Context.Initialize(new[] { existingContact });
            //prepare the new contact data
            Entity target = new Entity
            {
                LogicalName = "contact",
                Id = Guid.NewGuid()
            };
            target["emailaddress1"] = "test@test.com";

            fakeXrm.PluginExecutionContext.InputParameters.Add("Target", target);


            //Executing the plugin and asserting is there any invalid plugin execution
            PreValidateCContact plugin = new PreValidateCContact();
            InvalidPluginExecutionException ex = Assert.ThrowsException<InvalidPluginExecutionException>
                (() =>
                {
                    fakeXrm.Context.ExecutePluginWith<PreValidateCContact>(fakeXrm.PluginExecutionContext);
                });

        }

        /// <summary>
        /// Creating a contact with a whitespace email address.
        /// This test checks if the plugin throws an exception when trying to create a contact with an email address that contains only whitespace characters. 
        /// </summary>
        [TestMethod]
        public void expectPluginExceptionWhiteSpaceEmailProvided()
        {
            //initialize
            fakeXrm.PluginExecutionContext.MessageName = "Create";// Create event
            fakeXrm.PluginExecutionContext.Stage = 10;// Pre validate

            // Initilazing the existing contact
            Entity existingContact = new Entity { Id = Guid.NewGuid(), LogicalName = "contact", ["emailaddress1"] = "test@test.com" };
            fakeXrm.Context.Initialize(new[] { existingContact });
            //prepare the new contact data
            Entity target = new Entity
            {
                LogicalName = "contact",
                Id = Guid.NewGuid()
            };
            target["emailaddress1"] = " test@test.com ";

            fakeXrm.PluginExecutionContext.InputParameters.Add("Target", target);


            //Executing the plugin and asserting is there any invalid plugin execution
            PreValidateCContact plugin = new PreValidateCContact();
            InvalidPluginExecutionException ex = Assert.ThrowsException<InvalidPluginExecutionException>
                (() =>
                {
                    fakeXrm.Context.ExecutePluginWith<PreValidateCContact>(fakeXrm.PluginExecutionContext);
                });

        }

        /// <summary>
        /// Creating a contact with a valid email address.
        /// This test checks if the plugin allows the creation of a contact with a valid email address.
        /// The email address is checked against existing contacts to ensure it is unique.
        /// </summary>
        [TestMethod]
        public void AllowContact()
        {
            //initialize
            fakeXrm.PluginExecutionContext.MessageName = "Create";// create event
            fakeXrm.PluginExecutionContext.Stage = 10;// pre validate

            //Existing contact
            Entity existingContact = new Entity { Id = Guid.NewGuid(), LogicalName = "contact", ["emailaddress1"] = "test@test.com" };
            fakeXrm.Context.Initialize(new[] { existingContact });
            //prepare target data
            Entity target = new Entity
            {
                LogicalName = "contact",
                Id = Guid.NewGuid()
            };
            target["emailaddress1"] = "test@testTest.com";

            fakeXrm.PluginExecutionContext.InputParameters.Add("Target", target);


            //Executing plugin and asserting no exception thrown
            PreValidateCContact plugin = new PreValidateCContact();
            Exception ex = null;
            try
            {
                fakeXrm.Context.ExecutePluginWith(fakeXrm.PluginExecutionContext, plugin);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);

        }
    }
}
