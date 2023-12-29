echo "create table Script"
psql -d grechkadb -U postgres -W admin <<EOF 
CREATE TABLE guilds ( id serial primary key, lang varchar(6) DEFAULT ('us_US'), name_guild varchar, id_guild bigint); 
CREATE TABLE roles (id_guilds int references guilds (id), emoji varchar(150) not null, id_massage bigint not null, id_channel bigint not null,role_name varchar(70) not null, role_id bigint not null );
CREATE TABLE roomers_lobbys (id serial primary key, id_guilds int references guilds (id), id_lobby bigint);
CREATE TABLE rooms (
                    id serial primary key,
                    channelOwner bigint not null, 
                    name varchar not null, 
                    limit_vc int DEFAULT (0)
                    );
CREATE TABLE temp_rooms (
        id serial primary key,
        channel_room bigint not null,
        user_id bigint not null
    );
EOF
