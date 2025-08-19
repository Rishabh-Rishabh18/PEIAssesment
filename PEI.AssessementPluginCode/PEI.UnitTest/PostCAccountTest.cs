using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using PEI.Assesment.Plugins;
using System.Web.UI.WebControls;
using System.Linq;

namespace PEI.UnitTest
{
    /// <summary>
    /// Unit test cases for plugin PostCAccount.
    /// </summary>
    [TestClass]
    public class PostCAccountTest
    {
        // Intialize the Fake Xrm context and parameters
        internal FakeXrm fakeXrm = new FakeXrm();

        /// <summary>
        /// Creating contact with Account name as last name.
        /// Checking the last name of contact created is equal to Account name created 
        /// </summary>
        [TestMethod]
        public void CreateContactWithAccountName()
        {
            //Declaring plugin event message
            fakeXrm.PluginExecutionContext.MessageName = "Create";
            //Registered on post operation stage
            fakeXrm.PluginExecutionContext.Stage = 40;
            // async operation execution
            fakeXrm.PluginExecutionContext.Mode = 1;

            //prep for data
            fakeXrm.PluginExecutionContext.InputParameters["Target"] = new Entity { LogicalName = "account", Id = Guid.NewGuid(), ["name"] = "PEI Assesment"};
            //for assertion used later
            Entity account = fakeXrm.PluginExecutionContext.InputParameters["Target"] as Entity;

            //Executing plugin
            PostCreateAccount postCAccountTest = new PostCreateAccount();

            fakeXrm.Context.ExecutePluginWith(fakeXrm.PluginExecutionContext, postCAccountTest);
            
            //Asserting the result
            var contacts = fakeXrm.Context.CreateQuery("contact").ToList();
            Assert.AreEqual(1, contacts.Count);
            Entity createdContact = contacts.First();
            Assert.AreEqual("Default", createdContact["firstname"]);// checking the first name is Default
            Assert.AreEqual(account.GetAttributeValue<string>("name"), createdContact["lastname"]);// checking the last name is equal to Account name
            Assert.AreEqual((createdContact.GetAttributeValue<EntityReference>("parentcustomerid")).Id, account.Id);
        }

        [TestMethod]
        public void CreateContactWithOutAccountName()
        {
            fakeXrm.PluginExecutionContext.MessageName = "Create";
            fakeXrm.PluginExecutionContext.Stage = 40;
            fakeXrm.PluginExecutionContext.Mode = 1;

            fakeXrm.PluginExecutionContext.InputParameters["Target"] = new Entity { LogicalName = "account", Id = Guid.NewGuid() };

            Entity account = fakeXrm.PluginExecutionContext.InputParameters["Target"] as Entity;

            PostCreateAccount postCAccountTest = new PostCreateAccount();

            fakeXrm.Context.ExecutePluginWith(fakeXrm.PluginExecutionContext, postCAccountTest);

            var contacts = fakeXrm.Context.CreateQuery("contact").ToList();
            Assert.AreEqual(1, contacts.Count);

            Entity createdContact = contacts.First();
            Assert.AreEqual("Default", createdContact["firstname"]);
            Assert.AreEqual("Unknown", createdContact["lastname"]);
            Assert.AreEqual((createdContact.GetAttributeValue<EntityReference>("parentcustomerid")).Id, account.Id);
        }
    }
}
