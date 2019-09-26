import React, { Fragment } from 'react';
import { connect } from 'react-redux';
import { themeActions } from '../../_actions';
import Hamburger from 'react-hamburgers';

import AppMobileMenu from '../AppMobileMenu';

class HeaderLogo extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            active: false,
            mobile: false,
            activeSecondaryMenuMobile: false
        };

    }

    toggleEnableClosedSidebar = () => {
        let { enableClosedSidebar, setEnableClosedSidebar } = this.props;
        setEnableClosedSidebar(!enableClosedSidebar);
    }

    state = {
        openLeft: false,
        openRight: false,
        relativeWidth: false,
        width: 280,
        noTouchOpen: false,
        noTouchClose: false,
    };

    render() {
        let {
            enableClosedSidebar,
        } = this.props;
        return (
            <Fragment>
                <div className="app-header__logo">
                    <div className="logo-src" />
                    <div className="header__pane ml-auto">
                        <div onClick={this.toggleEnableClosedSidebar}>
                            <Hamburger
                                active={enableClosedSidebar}
                                type="elastic"
                                onClick={() => this.setState({ active: !this.state.active })}
                            />
                        </div>
                    </div>
                </div>
                <AppMobileMenu />
            </Fragment>
        )
    }
}


const mapStateToProps = state => {
    return ({
        enableClosedSidebar: state.theme.enableClosedSidebar,
        enableMobileMenu: state.theme.enableMobileMenu,
        enableMobileMenuSmall: state.theme.enableMobileMenuSmall,
    })
};

const mapDispatchToProps = dispatch => {
    return ({
        setEnableClosedSidebar: enable => dispatch(themeActions.setEnableClosedSidebar(enable)),
        setEnableMobileMenu: enable => dispatch(themeActions.setEnableMobileMenu(enable)),
        setEnableMobileMenuSmall: enable => dispatch(themeActions.setEnableMobileMenuSmall(enable)),

    })
};

export default connect(mapStateToProps, mapDispatchToProps)(HeaderLogo);