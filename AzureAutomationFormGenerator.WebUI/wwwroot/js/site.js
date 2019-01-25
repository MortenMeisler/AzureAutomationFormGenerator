// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function myFunction() {
    $("#spinner").html('<img id="loader-aa" src="~/../images/azureautomationIcon.svg" class="loader">');
}

// ***********************************************
// BEGIN - Parameter: string[]
// ***********************************************

//Create dynamic array variable for each string[] input type
let arrayInputs = {};
$(".btn.btn-primary.itemAdd").each(function (i, val) { arrayInputs[val.value] = [];});

function removeItemFromList(e) {
   
    //Remove item from array
    arrayInputs[e.data.inputKey].splice(arrayInputs[e.data.inputKey].indexOf("'" + $(this).parent().text() + "'"), 1);

    //remove list element from page
    $(this).parent().remove();

    if (arrayInputs[e.data.inputKey].length === 0)
    {
        //Reapply list is empty text
        $('<p class="empty-list" id=' + '"' + e.data.inputKey + '"' + '>The list is empty</p>').insertAfter("#itemTable_" + e.data.inputKey);
        
    }
    
}

$(".btn.btn-primary.itemAdd").on('click', function (event) {    
    event.preventDefault();

    //Get value of the input text box
    let inputValue = $("#itemInput_" + $(this).val()).val();
    let inputKey = $(this).val();

    //If input value from textbox is not empty and does not exists already
    if (inputValue.length > 0 && arrayInputs[inputKey].indexOf(inputValue) === -1) {
        //Get the id of the button clicked
        
        
        //Add value from specific button clicked to array
        arrayInputs[inputKey].push("'" + inputValue + "'");
        let keyvalue = inputKey + "_" + inputValue;
        //Add value to list on the page
        $("#formarray_" + inputKey).find("p.non-empty-list").append(
            '<li>' +
            '<button type="submit" id=' + '"' + keyvalue + '"' + 'class="btn btn-link itemRemove"><span class="fa fa-trash"></span></button>'
            + inputValue
            + '</li>'
        );
        // Assign event handler
        $(".btn.btn-link.itemRemove" + '#' + keyvalue).on("click", { inputKey: inputKey }, removeItemFromList);

        //Remove empty list
        $("p.empty-list" + '#' + inputKey).remove();

        //Store value to controller
        $("#" + inputKey).val("[" + arrayInputs[inputKey] + "]");

    }
 
});
// ***********************************************
// END - Parameter: string[]
// ***********************************************



 