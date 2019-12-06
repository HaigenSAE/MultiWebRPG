var shortID = require('shortid');
var Vector2 = require('./Vector2');
var Vector3 = require('./Vector3');
var PlayerStats = require('./PlayerStats');

module.exports = class Player {
    constructor() {
        this.username = '';
        this.id = shortID.generate().toString();
        this.position = new Vector3();
        this.position2D = new Vector2();
        this.playerStats = new PlayerStats();
        this.expAward = 0;
        this.minigameWon = '';
    }
}