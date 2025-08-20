// tests/harness.js
const { XrmMockGenerator } = require('xrm-mock')

/**
 * Initialize a fresh fake Xrm environment
 * @param {Object} attributes - Key-value pairs for field names and initial values
 * @returns {Object} formContext - Fake D365 form context
 */
function initFormContext(attributes = {}) {
  // Reset Xrm each time
  XrmMockGenerator.initialise()

  // Create attributes and keep references to their mocks
  const attrMocks = {}
  const ctrlMocks = {}

  for (const [fieldName, value] of Object.entries(attributes)) {
    let attribute

    // Handle different types of attributes
    if (value && typeof value === 'object' && value.type === 'optionset') {
      attribute = XrmMockGenerator.Attribute.createOptionSet(
        fieldName,
        value.value
      )
    } else {
      attribute = XrmMockGenerator.Attribute.createString(fieldName, value)
    }

    // Keep reference for spying later
    attrMocks[fieldName] = attribute

    // Also create a control mock for the attribute
    const control = XrmMockGenerator.Control.createString(fieldName)
    ctrlMocks[fieldName] = control
  }

  // Attach mocks to Xrm.Page for compatibility
  Xrm.Page.getAttribute = jest.fn((name) => attrMocks[name])
  Xrm.Page.getControl = jest.fn((name) => ctrlMocks[name])

  // Mock Navigation methods for dialogs
  global.Xrm.Navigation = {
    openAlertDialog: jest.fn().mockResolvedValue(true),
    openConfirmDialog: jest.fn().mockResolvedValue(true),
  }

  return Xrm.Page
}

/**
 * Build a fake execution context
 * @param {Object} formContext - The formContext from initFormContext
 * @param {Object} overrides - Additional props like getEventArgs mocks
 */
function buildExecutionContext(formContext, overrides = {}) {
  return {
    getFormContext: () => formContext,
    getEventArgs: () => ({
      preventDefault: jest.fn(),
    }),
    ...overrides,
  }
}

module.exports = {
  initFormContext,
  buildExecutionContext,
}
