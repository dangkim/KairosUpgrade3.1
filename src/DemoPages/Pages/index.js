import React, { Fragment } from 'react';
import { Route } from 'react-router-dom';

// NAVIGATION

import { LoginPage } from './LoginPage';
import { RegisterBrandPage } from './RegisterBrandPage';
import { LandingPage } from './LandingPage';
import { RegisterInfluencerPage } from './RegisterInfluencerPage';

// Layout
import { AppHeader } from '../../Layout/AppHeader';
import AppSidebar from '../../Layout/AppSidebar';
import AppFooter from '../../Layout/AppFooter';

const Pages = ({ match }) => {
    debugger;
    return (
        <Fragment>
            <Route path={`${match.url}/loginpage`} component={LoginPage} />
            <Route path={`${match.url}/registerbrandpage`} component={RegisterBrandPage} />
            <Route path={`${match.url}/landingpage`} component={LandingPage} />
            <Route path={`${match.url}/registerinfluencerpage`} component={RegisterInfluencerPage} />
            {/* <Route path={`${match.url}/loginpage`} component={LoginPage} />
        <Route path={`${match.url}/loginpage`} component={LoginPage} />
        <Route path={`${match.url}/loginpage`} component={LoginPage} /> */}
        </Fragment>
    )
};

export default Pages;