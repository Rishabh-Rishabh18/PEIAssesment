using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware.Pipeline;
using FakeXrmEasy.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.CodeActivities;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using FakeXrmEasy;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;

namespace PEI.UnitTest
{
    /// <summary>
    /// This class is used to set up a fake XRM context for unit testing purposes.
    /// It initializes the context, organization service, tracing service, plugin execution context, and workflow context.
    /// It uses the FakeXrmEasy library to create a simulated environment that mimics Dynamics 365 CRM behavior.
    /// The class provides properties to access these components, allowing for easy manipulation and testing of CRM.
    /// It also includes a method to set up the XRM faked context with specific configurations.
    /// </summary>
    internal class FakeXrm
    {
        /// <summary>
        /// Represents the XRM faked context used for unit testing.
        /// </summary>
        private IXrmFakedContext _xrmFakedContext;
        internal IXrmFakedContext Context
        {
            get { return _xrmFakedContext; }
            set { _xrmFakedContext = value; }
        }
        /// <summary>
        /// Represents the organization service used to interact with the CRM data.
        /// This service allows for CRUD operations on CRM entities and is essential for simulating Dynamics 365 CRM behavior in tests.
        /// </summary>
        private IOrganizationService _service;
        internal IOrganizationService OrganizationService
        {
            get { return _service; }
            set { _service = value; }
        }
        /// <summary>
        /// Represents the tracing service used for logging and debugging purposes.
        /// This service captures trace logs during plugin execution, which can be useful for diagnosing issues in unit tests.
        /// </summary>
        private IXrmFakedTracingService _tracingService;
        internal IXrmFakedTracingService TracingService
        {
            get { return _tracingService; }
            set { _tracingService = value; }
        }
        /// <summary>
        /// Represents the plugin execution context used during unit tests.
        /// This context contains information about the current plugin execution, such as input parameters, user ID, and other execution details.
        /// </summary>
        private XrmFakedPluginExecutionContext _pluginExecutionContext;
        internal XrmFakedPluginExecutionContext PluginExecutionContext
        {
            get { return _pluginExecutionContext; }
            set { _pluginExecutionContext = value; }
        }
        /// <summary>
        ///  Represents the workflow context used during unit tests.
        /// </summary>
        private XrmFakedWorkflowContext _workflowContext;
        internal XrmFakedWorkflowContext WorkflowContext
        {
            get { return _workflowContext; }
            set { _workflowContext = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeXrm"/> class.
        /// This constructor sets up the XRM faked context and initializes the organization service, tracing service, plugin execution context, and workflow context.
        /// </summary>
        public FakeXrm()
        {
            SetUpXrmFakedContext();
            this._service = this._xrmFakedContext.GetOrganizationService();
            this._tracingService = this._xrmFakedContext.GetTracingService();
            this._pluginExecutionContext = this._xrmFakedContext.GetDefaultPluginContext();
            this._pluginExecutionContext.InputParameters = new ParameterCollection();
            this._workflowContext = this._xrmFakedContext.GetDefaultWorkflowContext();
        }
        /// <summary>
        /// Sets up the XRM faked context with specific configurations.
        /// This method initializes the context with CRUD operations, fake message executors, and pipeline simulation
        /// </summary>
        internal void SetUpXrmFakedContext()
        {
            try
            {
                this._xrmFakedContext = MiddlewareBuilder
                .New().AddCrud().AddFakeMessageExecutors().AddPipelineSimulation(new PipelineOptions() { UsePluginStepAudit = true }).UsePipelineSimulation().UseCrud().UseMessages().SetLicense(FakeXrmEasyLicense.NonCommercial).Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
