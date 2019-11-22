var Skill = require('./Skill');

module.exports = class PlayerStats {
    constructor() {
        this.skills = [
            {
                skillName: "Cooking",
                curLevel: 1,
                curExp: 0
            },
            {
                skillName: "Mining",
                curLevel: 1,
                curExp: 0
            },
            {
                skillName: "Fishing",
                curLevel: 1,
                curExp: 0
            },
            {
                skillName: "Smithing",
                curLevel: 1,
                curExp: 0
            },
            {
                skillName: "Woodcutting",
                curLevel: 1,
                curExp: 0
            },
            {
                skillName: "FireMaking",
                curLevel: 1,
                curExp: 0
            }
          ];
    }
}