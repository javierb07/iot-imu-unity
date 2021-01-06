var websocketStarted = false;
var ws;
$("#add-button").click(function(event){
    var ip = $("#imu-ip").val();
    var url = $("#imu-url").val();
    if(ip && !url){
        let re = /(wss?):\/\/[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9]:[0-6]?[0-9]?[0-9]?[0-9]?[0-9]/;
        let re2 = /(wss?):\/\/localhost:?[0-6]?[0-9]?[0-9]?[0-9]?[0-9]?/
        if(re.test(ip) || re2.test(ip)){
            localStorage.setItem("ip", ip);
            ws = new WebSocket(ip);
            ws.onerror = function (event) {
                alert("Problem establishing communication with IMU WebSocket");
            }
        } else {
            alert("Invalid websocket IP");
        }
    } 
    if(!ip && url){
        let re3 = /(wss?):[\s\S]*/;
        if(re3.test(url)){
            localStorage.setItem("url", url);
            ws = new WebSocket(url);
            ws.onerror = function (event) {
                alert("Problem establishing communication with IMU WebSocket");
            }
        } else {
            alert("Invalid websocket URL");
        }
    }     
    if(ip && url){
        alert("Enter only URL or IP");
    }
    websocketWaiter();
    ws.send("imu");
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