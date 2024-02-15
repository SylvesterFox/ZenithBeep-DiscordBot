"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var express_1 = require("express");
var guildRouter_1 = require("./guildRouter");
var router = (0, express_1.Router)();
router.use('/guilds', guildRouter_1.default);
exports.default = router;
