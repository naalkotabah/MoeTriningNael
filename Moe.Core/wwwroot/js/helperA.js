//// System: auth
//// Base URL
//var baseUrl = "http://localhost:6969/api";
//var loginUrl = baseUrl + "/Auth/login";
//var commentsUrl = baseUrl + "/Comments";
//
//// DOM elements
//var loginForm = document.getElementById("loginForm");
//var emailInput = document.getElementById("emailInput");
//var passwordInput = document.getElementById("passwordInput");
//
//var connectToSocketBtn = document.getElementById("connectToSocketBtn");
//
//var sendMessageForm = document.getElementById("sendMessageForm")
//var senderNameInput = document.getElementById("senderNameInput")
//var senderMessageInput = document.getElementById("senderMessageInput")
//var sendMessageBtn = document.getElementById("sendMessageBtn")
//
//
//// Event listener for the login form submission
//loginForm.addEventListener("submit", function(e) {
//    e.preventDefault();
//    login();
//});
//
//// Login function
//async function login() {
//    const loginData = {
//        email: emailInput.value,
//        password: passwordInput.value
//    };
//
//    try {
//        var res = await fetch(
//            loginUrl,
//            {
//                method: "POST",
//                headers: {
//                    "accept": "text/plain",
//                    "Content-Type": "application/json"
//                },
//                body: JSON.stringify(loginData)
//            }
//        );
//        var resJson = await res.json();
//
//        var token = resJson.data;
//        console.log(token);
//        localStorage.setItem("token", token);
//
//        // Show the connectToSocketBtn after successful login
//        connectToSocketBtn.style.display = "inline";
//
//        // Hide the login form after successful login
//        loginForm.style.display = "none";
//    } catch (error) {
//        console.log(error);
//    }
//}
//
////System: sockets
//let connection
//function startSignalRConnection(){
//    const connection = new signalR.HubConnectionBuilder()
//        .withUrl("/chatHub", { accessTokenFactory: () => localStorage.getItem("token") })
//        .build();
//    
//    
//    connection.on("ReceiveMessage", function(user, message){
//        console.log(`${user}: ${message}`)
//    })
//
//    connection.start()
//        .then(() => {
//            console.log("SignalR connection established successfully.");
//        })
//        .catch(function (err) {
//            console.error("Error establishing SignalR connection:", err.toString());
//        });
//    
//    connectToSocketBtn.style.display = "none"
//    sendMessageForm.style.display = "inline"
//    
//    return connection;
//}
//connectToSocketBtn.addEventListener("click", function() {
//    connection = startSignalRConnection()
//})
//function sendMessage(){
//    const user = senderNameInput.value
//    const message = senderMessageInput.value
//    
//    connection.invoke("SendMessage", user, message).catch(function (err){
//        return console.error(err.toString())
//    })
//    
//    senderMessageInput.value = ""
//}
//sendMessageBtn.addEventListener("click", function(){
//    sendMessage()
//})