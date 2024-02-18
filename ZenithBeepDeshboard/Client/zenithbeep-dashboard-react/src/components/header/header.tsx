import logo from '../../static/logo.svg'
import kekba from '../../static/charm_menu-kebab.svg'
import "./Header.css"

function Header () {
    return (
            <header className='header'>
            <div className="container">
                <div className="header__row">
                    <div className="elements__first">
                        <div className='header__logo'>
                            <a href='#' className='logo__link'>
                                <img className='logo__image' src={logo} alt="logo" />
                                <span>ZenithBeep</span>
                            </a>
                        </div>
                        <nav className='header__nav__first'>
                            <ul>
                                <li>
                                    
                                    <a href="#!" className='docs'><span></span>DOCS</a>
                                </li>
                                <li>
                                    <a href="#!" className='commands'><span></span>COMMANDS</a>
                                    
                                </li>
                                <li className='list__header'>
                                    <a href="#!" className='support_us'><span></span>SUPPORT US</a>
                                
                                </li>
                            </ul>
                        </nav>
                    </div>
                    <div className='elements__second'>
                        <nav className="header__nav__second">
                            <ul>
                                <li>
                                    <span className='button'><img src={kekba} alt="" /></span>
                                </li>
                                <li>
                                    <a href="#!" className='Sign_in'><span></span>SIGN IN</a>
                                </li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>
        </header>
    )
}

export default Header;