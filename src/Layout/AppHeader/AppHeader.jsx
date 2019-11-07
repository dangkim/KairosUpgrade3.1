import React, { Fragment } from 'react';
import cx from 'classnames';

import { connect } from 'react-redux';

//import ReactCSSTransitionGroup from 'react-transition-group/TransitionGroup'; 

import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';

import HeaderLogo from '../AppLogo';

import { SearchBox } from './Components/SearchBox/SearchBox';
import { UserBox } from './Components/UserBox/UserBox';

// transitionname="HeaderAnimation"
//                     transitionAppearTimeout={1500}
//                     transitionEnterTimeout={1000}
//                     transitionLeaveTimeout={1000}

class AppHeader extends React.Component {

    constructor(props) {
        super(props);

        this.state = {
            showList: true,
            highlightedHobby: false
        };

        this.sendData = this.sendData.bind(this);
    }

    switch = () => {
        this.setState(prevState => ({
            showList: !prevState.showList
        }));
    };

    listSwitch = () => {
        this.setState(state => ({
            highlightedHobby: !state.highlightedHobby
        }));
    };

    sendData = (searchValue) => {
        // Back to first tab
        debugger;
        this.props.parentCallback(searchValue);
    }

    render() {
        let {
            headerBackgroundColor,
            enableMobileMenuSmall,
            enableHeaderShadow
        } = this.props;

        const fullName = localStorage.getItem("brandFullName");
        return (
            <Fragment>
                <CSSTransition timeout={1500} unmountOnExit
                    in
                    classNames="HeaderAnimation"
                    appear>
                    <div className={cx("app-header", headerBackgroundColor, { 'header-shadow': enableHeaderShadow })}>
                        <HeaderLogo />
                        <div className={cx("app-header__content", { 'header-mobile-open': enableMobileMenuSmall })}>
                            <div className="app-header-left">
                                <SearchBox handlerFromParent={this.sendData} />
                            </div>
                            <div className="app-header-right">
                                <UserBox FullName={fullName}/>
                            </div>
                        </div>
                    </div>
                </CSSTransition>
            </Fragment>
        );
    }
}

const mapStateToProps = state => {
    return ({
        enableHeaderShadow: state.theme.enableHeaderShadow,
        closedSmallerSidebar: state.theme.closedSmallerSidebar,
        headerBackgroundColor: state.theme.headerBackgroundColor,
        enableMobileMenuSmall: state.theme.enableMobileMenuSmall,
    })
};

const mapDispatchToProps = dispatch => ({});

const connectedAppHeader = connect(mapStateToProps, mapDispatchToProps)(AppHeader);
export { connectedAppHeader as AppHeader };
//export default connect(mapStateToProps, mapDispatchToProps)(connectedAppHeader);