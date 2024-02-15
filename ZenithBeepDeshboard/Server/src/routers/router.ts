import { Router } from "express";
import guildrouter from "./guildRouter";
const router = Router();

router.use('/guilds', guildrouter)

export default router;