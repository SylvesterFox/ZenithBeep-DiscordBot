import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import { HomePage } from './pages/HomePage';
import { GuildsPage } from './pages/GuildsPage';
import { CommonGuildPage } from './pages/CommonGuildPage';
import { MusicGuildPage } from './pages/MusicGuildPage';
import { CommandsGuildPage } from './pages/CommandsGuildPage';
import { GuildContext } from './utils/context/GuildContext';


function App() {
  const [guildId, setGuildId] = useState('');
  const updateGuildId = (id: string) => setGuildId(id)
  return (
    <GuildContext.Provider value={{ guildId, updateGuildId }}>
      <Routes>
        
          <Route path='/' element={<HomePage />} />
          <Route path='/guilds' element={<GuildsPage />} />
          <Route path='/guild/dashboard' element={<CommonGuildPage />} />
          <Route path='/guild/dashboard/common' element={<CommonGuildPage />} />
          <Route path='/guild/dashboard/music' element={<MusicGuildPage />} />
          <Route path='/guild/dashboard/commands' element={<CommandsGuildPage />} />
      
      </Routes>
    </GuildContext.Provider>
    
  )
  
      
}

export default App;
