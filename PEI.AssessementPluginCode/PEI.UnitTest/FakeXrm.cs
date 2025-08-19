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
    internal class FakeXrm
    {
        private IXrmFakedContext _xrmFakedContext;
        internal IXrmFakedContext Context
        {
            get { return _xrmFakedContext; }
            set { _xrmFakedContext = value; }
        }

        private IOrganizationService _service;
        internal IOrganizationService OrganizationService
        {
            get { return _service; }
            set { _service = value; }
        }

        private IXrmFakedTracingService _tracingService;
        internal IXrmFakedTracingService TracingService
        {
            get { return _tracingService; }
            set { _tracingService = value; }
        }

        private XrmFakedPluginExecutionContext _pluginExecutionContext;
        internal XrmFakedPluginExecutionContext PluginExecutionContext
        {
            get { return _pluginExecutionContext; }
            set { _pluginExecutionContext = value; }
        }

        private XrmFakedWorkflowContext _workflowContext;
        internal XrmFakedWorkflowContext WorkflowContext
        {
            get { return _workflowContext; }
            set { _workflowContext = value; }
        }
        public FakeXrm()
        {
            SetUpXrmFakedContext();
            this._service = this._xrmFakedContext.GetOrganizationService();
            this._tracingService = this._xrmFakedContext.GetTracingService();
            this._pluginExecutionContext = this._xrmFakedContext.GetDefaultPluginContext();
            this._pluginExecutionContext.InputParameters = new ParameterCollection();
            this._workflowContext = this._xrmFakedContext.GetDefaultWorkflowContext();
        }
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
