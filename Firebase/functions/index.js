const functions = require("firebase-functions");
const admin = require("firebase-admin");

// Create and Deploy Your First Cloud Functions
// https://firebase.google.com/docs/functions/write-firebase-functions

exports.HelloWorld = functions.https.onRequest((request, response) => {
    response.send("Hello from Firebase!");
});

admin.initializeApp();

// Gatilho HTTP que, quando acionado por uma solicitação, verifica cada canal do banco de dados para excluir os expirados.
exports.RemoveOldChannels = functions.https.onRequest((request, response) => {
    const TimeNow = Date.now();
    const Reference = admin.database().ref("/canais/");
    
    Reference.once("value", (snapshot) => {
        snapshot.forEach((child) => {
            if ((Number(child.val()["timestamp"]) + 360 /* 12 * 60 * 60 * 1000 */ /* + Number(child.val()['duration']) */) <= TimeNow) {
                child.ref.set(null);
            }
        });
    });

    return res.status(200).end();
});