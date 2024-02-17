import '../style/HomePage.css'
import logo from '../style/static/logo.svg'

export const HomePage = () => (
    <header className='header'>
        <div className="container">
            <div className="elements_1">
                <div className='logo'>
                    <a href='#' className='logo_link'>
                        <img className='logo_image' src={logo} alt="logo" />
                        <span className='logo_name'>ZenithBeep</span>
                    </a>
                </div>
                <nav className='nav'>
                    <ul className='list_nav'>
                        <li className='items_link'>
                            <a href="#" className='link_nav'>DOCS</a>
                        </li>
                        <li className='items_link'>
                            <a href="#" className='link_nav'>COMMANDS</a>
                        </li>
                        <li className='items_link'>
                            <a href="#" className='link_nav'>SUPPORT US</a>
                        </li>
                    </ul>
                </nav>
            </div>
            <div className='elements_2'>
                <nav className="nav_2">
                    <ul className="list_nav">
                        <li className="items_link">
                            <a href="" className="link_nav"></a>
                        </li>
                        <li className="items_link">
                            <a href="" className="link_nav">SIGN IN</a>
                        </li>
                    </ul>
                </nav>
            </div>
        </div>
    </header>
);