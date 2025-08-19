var Contact = Contact || {}

Contact.Form = {
  /**
   * Function to set Email or Business Phone as required based on Preferred Method of Contact field value
   * @param {object} executionContext
   */
  OnChangePCMethod: function (executionContext) {
    let formContext = executionContext?.getFormContext()
    console.log('OnChangePCMethod called')
    let preferedMC = formContext
      .getAttribute('preferredcontactmethodcode')
      ?.getValue()
    console.log('Preferred Method of Contact value found:', preferedMC)
    if (preferedMC) {
      console.log('Set required feild based on value')
      if (preferedMC === 2) {
        console.log('Setting Email as required')
        // If Preferred Method of Contact is Email set Email as Required
        formContext.getAttribute('emailaddress1')?.setRequiredLevel('required')
        formContext.getAttribute('telephone1')?.setRequiredLevel('none')
      } else if (preferedMC === 3) {
        console.log('Setting Phone as required')
        // If Preferred Method of Contact is Phone set Business Phone as Required
        formContext.getAttribute('telephone1')?.setRequiredLevel('required')
        formContext.getAttribute('emailaddress1')?.setRequiredLevel('none')
      }
      // Else do nothing and remove the required field level from other fields
      else {
        console.log('No specific Preferred Method of Contact set')
        formContext.getAttribute('telephone1')?.setRequiredLevel('none')
        formContext.getAttribute('emailaddress1')?.setRequiredLevel('none')
      }
    } else {
      console.log('Value not present for PMC')
    }
  },

  /**
   * Function to validate either Email or Phone field is populated or not. If not prevent save and show an alert to user
   * @param {object} executionContext
   */
  ValidatePCMFieldsDetails: function (executionContext) {
    let formContext = executionContext?.getFormContext()
    let eventArgs = executionContext.getEventArgs()
    if (
      !formContext.getAttribute('telephone1').getValue() &&
      !formContext.getAttribute('emailaddress1').getValue()
    ) {
      // checking either any of the field values are empty
      eventArgs.preventDefault() // prevent save action as validation fails
      var alertStrings = {
        confirmButtonLabel: 'Ok',
        text: 'Either Bussines Phone or Email needs to be entered',
        title: 'Alert',
      }
      var alertOptions = { height: 120, width: 260 }
      // showing Alert Dialog with the validation message
      Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
        function (success) {
          //console.log('Alert dialog closed')
        },
        function (error) {
          console.log(error.message)
        }
      )
    }
  },
}

if (typeof module !== 'undefined' && module.exports) {
  module.exports = Contact
}
