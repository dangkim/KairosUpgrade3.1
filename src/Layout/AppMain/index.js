import { BrowserRouter as Router, Route, Redirect } from 'react-router-dom';
import React, { Suspense, lazy, Fragment } from 'react';
import { history } from '../../_helpers';
import {
    ToastContainer,
} from 'react-toastify';
import Loader from 'react-loader-spinner'
const Dashboards = lazy(() => import('../../DemoPages/Dashboards'));

const connectedWidgets = lazy(() => import('../../DemoPages/Widgets'));
//const Elements = lazy(() => import('../../DemoPages/Elements'));
//const Components = lazy(() => import('../../DemoPages/Components'));
const Pages = lazy(() => import('../../DemoPages/Pages'));
//const RegisterBrandPage = lazy(() => import('../../DemoPages/RegisterBrandPage'));
//const RegisterInfluencerPage = lazy(() => import('../../DemoPages/RegisterInfluencerPage'));
//const Charts = lazy(() => import('../../DemoPages/Charts'));
//const Forms = lazy(() => import('../../DemoPages/Forms'));
//const Tables = lazy(() => import('../../DemoPages/Tables'));

const AppMain = () => {
    return (
        <Fragment>

            {/* Components */}

            {/* <Suspense fallback={
                <div className="loader-container">
                    <div className="loader-container-inner">
                        <h6 className="mt-5">
                            Please wait while we load all the Components examples
                <small>Because this is a demonstration we load at once all the Components examples. This wouldn't happen in a real live app!</small>
                        </h6>
                    </div>
                </div>
            }>
                <Route path="/components" component={Components} />
            </Suspense> */}

            <Suspense fallback={
                <div className="loader-container">
                    <div className="loader-container-inner">
                        <Loader
                            type="CradleLoader"
                            color="#00BFFF"
                            height={100}
                            width={100}
                        />
                    </div>
                </div>
            }>
                <Route path="/pages" component={Pages} />
            </Suspense>

            {/* <Suspense fallback={
    <div className="loader-container">
        <div className="loader-container-inner">
            <h6 className="mt-5">
                Please wait while we load all the Components examples
                <small>Because this is a demonstration we load at once all the Components examples. This wouldn't happen in a real live app!</small>
            </h6>
        </div>
    </div>
}>
    <Route path="/registerinfluencerpage" component={RegisterInfluencerPage} />
</Suspense> */}

            {/* <Suspense fallback={
    <div className="loader-container">
        <div className="loader-container-inner">
            <h6 className="mt-5">
                Please wait while we load all the Components examples
                <small>Because this is a demonstration we load at once all the Components examples. This wouldn't happen in a real live app!</small>
            </h6>
        </div>
    </div>
}>
    <Route path="/registerbrandpage" component={RegisterBrandPage} />
</Suspense> */}

            {/* Forms */}

            {/* <Suspense fallback={
    <div className="loader-container">
        <div className="loader-container-inner">
            <h6 className="mt-5">
                Please wait while we load all the Forms examples
                <small>Because this is a demonstration we load at once all the Forms examples. This wouldn't happen in a real live app!</small>
            </h6>
        </div>
    </div>
}>
    <Route path="/forms" component={Forms} />
</Suspense> */}

            {/* Charts */}

            {/* <Suspense fallback={
    <div className="loader-container">
        <div className="loader-container-inner">
            <h6 className="mt-3">
                Please wait while we load all the Charts examples
                <small>Because this is a demonstration we load at once all the Charts examples. This wouldn't happen in a real live app!</small>
            </h6>
        </div>
    </div>
}>
    <Route path="/charts" component={Charts} />
</Suspense> */}

            {/* Tables */}

            {/* <Suspense fallback={
    <div className="loader-container">
        <div className="loader-container-inner">
            <h6 className="mt-5">
                Please wait while we load all the Tables examples
                <small>Because this is a demonstration we load at once all the Tables examples. This wouldn't happen in a real live app!</small>
            </h6>
        </div>
    </div>
}>
    <Route path="/tables" component={Tables} />
</Suspense> */}

            {/* Elements */}

            {/* <Suspense fallback={
                <div className="loader-container">
                    <div className="loader-container-inner">
                        <h6 className="mt-3">
                            Please wait while we load all the Elements examples
                <small>Because this is a demonstration we load at once all the Elements examples. This wouldn't happen in a real live app!</small>
                        </h6>
                    </div>
                </div>
            }>
                <Route path="/elements" component={Elements} />
            </Suspense> */}

            {/* Dashboard Widgets */}

            <Suspense fallback={
                <div className="loader-container">
                    <div className="loader-container-inner">
                        <Loader
                            type="CradleLoader"
                            color="#00BFFF"
                            height={100}
                            width={100}
                        />
                    </div>
                </div>
            }>
                <Route path="/widgets" component={connectedWidgets} />
            </Suspense>

            {/* Dashboards */}

            {/* <Suspense fallback={
                <div className="loader-container">
                    <div className="loader-container-inner">
                    <Loader
                            type="CradleLoader"
                            color="#00BFFF"
                            height={100}
                            width={100}
                            timeout={3000} //3 secs
                        />
                    </div>
                </div>
            }>
                <Route path="/dashboards" component={Dashboards} />
            </Suspense> */}

            <Route exact path="/" render={() => (
                <Redirect to="/pages/landingpage" />
            )} />
            <ToastContainer />
        </Fragment>
    )
};

export default AppMain;