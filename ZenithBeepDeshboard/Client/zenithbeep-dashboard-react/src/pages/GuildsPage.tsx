import { useContext } from "react"
import { GuildContext } from "../utils/context/GuildContext"
import { mockGuilds } from "../__mocks__/guilds"
import { useNavigate } from "react-router-dom"

export const GuildsPage = () => {
    const navigate = useNavigate();
    const { updateGuildId } = useContext(GuildContext);
    return (
        <div>
            <ul>
                {
                    mockGuilds.map((guild) => (
                        <li onClick={() => {
                            updateGuildId(guild.id)
                            navigate('/guild/dashboard')
                        }}>{guild.name}</li>
                    ))
                }
            </ul>
        </div>
    )
}