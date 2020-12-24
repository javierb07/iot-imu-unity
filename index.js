var websocketStarted = false;
var ws;
$("#add-button").click(function(event){
    console.log("clicked");
    var url = $("#imu-url").val();
    console.log(url);
    let re = /(wss?):\/\/[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9]:[0-6]?[0-9]?[0-9]?[0-9]?[0-9]/;
    if(re.test(url)){
    ws = new WebSocket(url);
    ws.onerror = function (event) {
        alert("Problem establishing communication with IMU WebSocket")
    }
    websocketWaiter();
    ws.onmessage = function (event) {
        var data = event.data.split(',');
        document.getElementById('Time').innerHTML = data[0];
        document.getElementById('Acc x').innerHTML = data[1];
        document.getElementById('Acc y').innerHTML = data[2];
        document.getElementById('Acc z').innerHTML = data[3];
        document.getElementById('Quat w').innerHTML =data[4];
        document.getElementById('Quat x').innerHTML =data[5];
        document.getElementById('Quat y').innerHTML =data[6];
        document.getElementById('Quat z').innerHTML =data[7];
    };
    } else {
        alert("Invalid websocket IP");
    }
});	
    


function websocketWaiter(){
    setTimeout(function(){
		if (ws.readyState === 1) {
                console.log("Connection is made")
            } else {
                console.log("wait for connection...")
                websocketWaiter();
            }
        }, 5); // wait 5 milisecond for the connection...
};