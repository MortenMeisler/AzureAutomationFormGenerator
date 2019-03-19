// The following sample code uses modern ECMAScript 6 features 
// that aren't supported in Internet Explorer 11.
// To convert the sample for environments that do not support ECMAScript 6, 
// such as Internet Explorer 11, use a transpiler such as 
// Babel at http://babeljs.io/. 
//
// See Es5-chat.js for a Babel transpiled version of the following code:

//Create connection and start it
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//Start
//connection.start().catch(err => console.error(err.toString())).then(function(){
//    connection.invoke('getConnectionId')
//        .then(function (connectionId) {
//            // Send the connectionId to controller
//            console.log("connectionID: " + connectionId);
//            $("#signalRconnectionId").attr("value", connectionId);
//        });
//});
connection.start().catch(err => console.error(err.toString()));
//Reconnect on disconnect
$(connection).bind("onDisconnect", function (e, data) {
    connection.start().catch(err => console.error(err.toString()));
});

//$(connection).bind("onDisconnect", function (e, data) {
//    connection.start().catch(err => console.error(err.toString())).then(function () {
//        connection.invoke('getConnectionId')
//            .then(function (connectionId) {
//                // Send the connectionId to controller
//                console.log("Reconnecting... connectionID: " + connectionId);
//                $("#signalRconnectionId").attr("value", connectionId);
//            });
//    });
//});





//Send Message - Signal method invoked from server
connection.on("initMessage", (message) => {

    console.log("We got signal! and the message is: " + message);

    //Update paragraph tag with the message sent
    $("#jobmessage").html(message);

});

//Send Status - Signal method invoked from server
connection.on("initStatus", (status) => {

    //Update paragraph tag with status
    $("#jobstatus").html(status);
});


//Signal method invoked from server
connection.on("initErrorMessage", (message) => {

    //Update paragraph tag with the message sent
    $("#errormessage").html(message);

});

//Submit button clicked event listener
document.getElementById("submitButton").addEventListener("click", function (event) {

    
});
