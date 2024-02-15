import { Entity, Column, PrimaryColumn, NumericType} from "typeorm";

@Entity({ name: 'public.Guilds' })
export class Guilds {
    @PrimaryColumn()
    Id: number

    @Column({ type: "numeric" })
    guildId: string

    @Column()
    Lang: string

    @Column()
    Prefix: string

}