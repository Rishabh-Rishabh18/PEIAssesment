using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PEI.Assesment.Plugins;
using Microsoft.Xrm.Sdk;

using System.Linq;
using FakeXrmEasy.Plugins;

namespace PEI.UnitTest
{
    [TestClass]
    public class PreValidateContactTest
    {
        // intiating the fake xrm properties
        internal FakeXrm fakeXrm = new FakeXrm();
        
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
