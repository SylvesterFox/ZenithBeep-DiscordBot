import { useContext } from "react"
import { GuildContext } from "../utils/context/GuildContext"

export const CommonGuildPage = () => {
    const { guildId } = useContext(GuildContext);
    return <div>CommonPageDashboard {guildId}</div>
}