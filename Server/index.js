var io = require('socket.io')(process.env.PORT || 52300);
//Custom Classes
var Player = require('./Classes/Player.js');

// Firebase App (the core Firebase SDK) is always required and
// must be listed before other Firebase SDKs
var firebase = require("firebase/app");
// Add the Firebase products that you want to use
require("firebase/auth");
require("firebase/firestore");

var admin = require("firebase-admin");
var serviceAccount = require("./ServiceKey.json");

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://clubrpg-69.firebaseio.com"
});

// Your web app's Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyBmgbR7A-cR7IP99gCCSni69VMEVeZ2dP4",
    authDomain: "clubrpg-69.firebaseapp.com",
    databaseURL: "https://clubrpg-69.firebaseio.com",
    projectId: "clubrpg-69",
    storageBucket: "clubrpg-69.appspot.com",
    messagingSenderId: "126051118526",
    appId: "1:126051118526:web:d6f7a70764cdb147c6ddef",
    measurementId: "G-F4R9SG7NST"
};
// Initialize Firebase
firebase.initializeApp(firebaseConfig);

console.log('Server Started');

var players = [];
var sockets = [];

var ExpHigh = 300;
var ExpLow = 100;

var db = admin.firestore();
var usersRef = db.collection('users');

io.on('connection', function(socket){
    console.log('Connection Made');
    var player = new Player();
    var thisPlayerID = player.id;

    players[thisPlayerID] = player;
    sockets[thisPlayerID] = socket;

    //login using existing id
    socket.on('loginClient', function(data){     
        if(data.loginID != '')
        {
            console.log(data.loginID);
            firebase.auth().signInWithEmailAndPassword(data.loginID, data.password).then(function(logUser) 
            {
                var authUID = logUser.user.uid;
                console.log(authUID);
                usersRef.doc(authUID.toString()).get().then(doc => 
                {
                    if(!doc.exists)
                    {
                        console.log('no such player exists');
                        socket.emit('registered', {id: thisPlayerID});
                    }
                    else if(players[authUID] != null)
                    {
                        console.log('player already in game');
                        socket.emit('alreadyLoggedIn');
                    }
                    else
                    {
                        console.log('attempting to load: ', authUID);
                        var myData = doc.data();
                        delete players[thisPlayerID];
                        delete sockets[thisPlayerID];
                        //setup new player
                        players[authUID] = player;
                        sockets[authUID] = socket;
                        thisPlayerID = authUID;
                        //assign data
                        player = myData;
                        player.id = authUID;
                        //console.log('loaded player: ', player);
                        socket.emit('loggedIn', {id: thisPlayerID});
                    }  
                });
            }).catch(function(error) {
                var errorCode = error.code;
                switch(error.code)
                {
                    case 'auth/user-not-found':
                        socket.emit('userNotFound');
                    break;
                    case 'auth/wrong-password':
                        socket.emit('incorrectPassword');
                    break;
                    case 'auth/email-already-in-use':
                    socket.emit('alreadyExists');
                    console.log('alreadyExists');
                }
                console.log(error.code);
            });           
        }
    });

    //register new user
    socket.on('registerClient', function(data){
        var regoID = data.regoID.toString();
        if(regoID != '')
        {
            firebase.auth().createUserWithEmailAndPassword(data.regoID, data.password).then(function(regUser) 
            {
                console.log(regUser.user.uid);
                var authUID = regUser.user.uid;
                usersRef.doc(authUID.toString()).get().then(doc => {
                    if(!doc.exists)
                    {   
                        console.log('regoID = ', authUID);
                        if(authUID != '')
                        { 
                            delete players[thisPlayerID];
                            delete sockets[thisPlayerID];
                            //setup new player
                                players[authUID] = player;
                                sockets[authUID] = socket;
                                //assign data
                                player.id = authUID;
                                thisPlayerID = authUID;
                                usersRef.doc(authUID.toString()).set({
                                    username: player.username.toString(),
                                    playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                                    position: JSON.parse(JSON.stringify(player.position))
                                });          
                            }
                            else
                            {
                                usersRef.doc(player.id.toString()).set({
                                    username: player.username.toString(),
                                    playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                                    position: JSON.parse(JSON.stringify(player.position))
                                });
                            }
                            console.log('Created player: ', player);         
                            socket.emit('registered', {id: thisPlayerID});
                            }
                            else
                            {
                                console.log(regoID , 'already exists');
                                socket.emit('alreadyExists');
                            }
                    })
                }).catch(function(error) {
                // Handle Errors here.
                var errorCode = error.code;
                switch(error.code)
                {
                    case 'auth/user-not-found':
                        socket.emit('userNotFound');
                    break;
                    case 'auth/wrong-password':
                        socket.emit('incorrectPassword');
                    break;
                    case 'auth/email-already-in-use':
                    socket.emit('alreadyExists');
                    console.log('alreadyExists');
                }
                console.log(error);
            });     
        }             
    });

    //bring them in
    socket.on('enterGame', function(data){
        console.log('entering game');
        //Tell the client that this is our id for the server
        socket.emit('register', {id: thisPlayerID});
        //console.log(player);
        socket.emit('spawn', player); //Tell myself I have spawned
        socket.broadcast.emit('spawn', player); //Tell others I have spawned
    });

    //Load players and playerinfo
    for(var playerID in players){
        if(playerID != thisPlayerID){
            socket.emit('spawn', players[playerID]);
        }
    }

    socket.on('newChatMessage', function(data){
        socket.broadcast.emit('receiveChatMessage', data.text);
    });

    //Positional Data from Client
    socket.on('updatePosition', function(data) {
        player.position.x = data.position.x;
        player.position.y = data.position.y;
        player.position.z = data.position.z;

        socket.broadcast.emit('updatePosition', player);
    });

    //pull info from database
    socket.on('loadData', function(data) {
        console.log('load from db'); 
        usersRef.doc(player.id.toString()).get().then(doc => {
            if(!doc.exists)
            {
                console.log('no such doc exists');
            }
            var myData = doc.data();
            player.playerStats = myData.playerStats;
        })
        .catch(err => {
            console.log('error getting doc', err);
        });

        
        socket.broadcast.emit('loadData', player);
    });

    //save info to database
    socket.on('saveData', function(data) {
        console.log('save to db');
        //console.log(data); 
        for(var i = 0; i < 6; i ++)
        {
            player.playerStats.skills[i] = data.playerStats.skills[i];
            
        }
        usersRef.doc(player.id.toString()).set({
                username: player.username.toString(),
                playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                position: JSON.parse(JSON.stringify(player.position))
        });
        socket.emit('saveData', player);
    });


    socket.on('successfulMinigame', function(data){
        //recieve minigame type, send back random winnings (exp)
        console.log('successful Minigame'); 
        player.minigameWon = data.minigameWon;
        player.expAward = Math.random() * (ExpHigh - ExpLow) + ExpLow;
        console.log(player.expAward, ' exp for ', player.minigameWon);
        socket.emit('successMinigame', player);
        console.log('broadcast sent to client');
        player.expAward = 0;
    });

    socket.on('disconnect', function(){
        firebase.auth().signOut();
        console.log('Player disconnected');
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit('disconnected', player);
    });
});