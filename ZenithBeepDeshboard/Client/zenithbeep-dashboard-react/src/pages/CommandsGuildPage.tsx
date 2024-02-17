import { useContext } from "react";
import { GuildContext } from "../utils/context/GuildContext";

export const CommandsGuildPage = () => {
    const { guildId } = useContext(GuildContext);
    return <div>CommandsPageDashboard {guildId}</div>
}