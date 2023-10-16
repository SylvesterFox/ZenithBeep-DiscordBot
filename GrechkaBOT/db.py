import psycopg2
import argparse

conn = psycopg2.connect(host="localhost", database="grechkadb", user="postgres", password="0270")



def add(arg):
    string = []
    if arg.create:
        guild_create = """CREATE TABLE guilds (
            id serial primary key, 
            lang varchar(6) DEFAULT ('us_US'), 
            name_guild varchar, 
            id_guild bigint
            )"""
        roles_create = """CREATE TABLE roles (
            id_guilds int references guilds (id), 
            emoji varchar(150) not null, 
            id_massage bigint not null, 
            id_channel bigint not null,
            role_name varchar(70) not null, 
            role_id bigint not null 
        )"""

        string.append(guild_create)
        string.append(roles_create)
        create(string)
    elif arg.drop:
        roles_drop = "DROP TABLE IF EXISTS roles CASCADE;"
        guild_drop = "DROP TABLE IF EXISTS guilds CASCADE;"
        string.append(roles_drop)
        string.append(guild_drop)
        drop(string)


def create(obj):
    cursor = conn.cursor()
    for i in obj:
        cursor.execute(i)
        conn.commit()

def drop(obj):
    cursor = conn.cursor()
    for i in obj:
        cursor.execute(i)
        conn.commit()


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--create', help="Create table", action='store_true')
    parser.add_argument('--drop', help="Drop table", action='store_true')
    args = parser.parse_args()
    add(args)
   
    
