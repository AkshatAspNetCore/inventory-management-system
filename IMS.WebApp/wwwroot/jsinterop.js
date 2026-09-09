function preventFormSubmission(formId) {

    document.getElementById(formId).addEventListener('keydown', function (event) {

        if (event.key == "Enter") {
            event.preventDefault(); // prevent form submission
            return false;  
        }

    });

}