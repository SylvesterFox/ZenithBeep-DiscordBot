import { Router } from "express";
import { Request, Response } from "express"
import { AppDataSource } from "../data-source";
import { Guilds } from "../entity/guild.entity";
import { NumericType } from "typeorm";


let guildrouter = Router();

guildrouter.get("/:id", async function (req: Request, res: Response) {
    const results = await AppDataSource.getRepository(Guilds).findOneBy({
        Id: Number(req.params.id)
    })
    res.json(results)
})

guildrouter.get("/guild/:guildId", async function (req: Request, res: Response) {
    const results = await AppDataSource.getRepository(Guilds).findOneBy({
        guildId: req.params.guildId
    })
    res.json(results)
})

guildrouter.get("/", async function (req: Request, res: Response) {
    const results = await AppDataSource.getRepository(Guilds).find()
    res.json(results)
})

export default guildrouter