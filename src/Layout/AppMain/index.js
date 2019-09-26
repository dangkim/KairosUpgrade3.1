import { BrowserRouter as Router, Route, Redirect } from 'react-router-dom';
import React, { Suspense, lazy, Component, Fragment } from 'react';
import { history } from '../../_helpers';
import {
    ToastContainer,
} from 'react-toastify';

const Dashboards = lazy(() => import('../../DemoPages/Dashboards'));

const Widgets = lazy(() => import('../../DemoPages/Widgets'));
const Elements = lazy(() => import('../../DemoPages/Elements'));
const Components = lazy(() => import('../../DemoPages/Components'));
const Pages = lazy(() => import('../../DemoPages/Pages'));
//const RegisterBrandPage = lazy(() => import('../../DemoPages/RegisterBrandPage'));
//const RegisterInfluencerPage = lazy(() => import('../../DemoPages/RegisterInfluencerPage'));
//const Charts = lazy(() => import('../../DemoPages/Charts'));
//const Forms = lazy(() => import('../../DemoPages/Forms'));
//const Tables = lazy(() => import('../../DemoPages/Tables'));

class AppMain extends Component {

    constructor(props) {
        super(props);

    }

    render() {
        const { cookies } = this.props;
        debugger;
        return (
            <Fragment>

                {/* Components */}

                <Suspense fallback={
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
                </Suspense>

                <Suspense fallback={
                    <div className="loader-container">
                        <div className="loader-container-inner">
                            <h6 className="mt-5">
                                Please wait while we load all the Components examples
                <small>Because this is a demonstration we load at once all the Components examples. This wouldn't happen in a real live app!</small>
                            </h6>
                        </div>
                    </div>
                }>
                    <Route path="/pages" render={(props) => <Pages {...props} cookies={props.cookies} />} />
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

                <Suspense fallback={
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
                </Suspense>

                {/* Dashboard Widgets */}

                <Suspense fallback={
                    <div className="loader-container">
                        <div className="loader-container-inner">
                            <h6 className="mt-3">
                                Please wait while we load all the Dashboard Widgets examples
                <small>Because this is a demonstration we load at once all the Dashboard Widgets examples. This wouldn't happen in a real live app!</small>
                            </h6>
                        </div>
                    </div>
                }>
                    <Route path="/widgets" render={() => (<Widgets cookies={this.props.cookies} />)} />
                </Suspense>

                {/* Dashboards */}

                <Suspense fallback={
                    <div className="loader-container">
                        <div className="loader-container-inner">
                            <h6 className="mt-3">
                                Please wait while we load all the Dashboards examples
                <small>Because this is a demonstration, we load at once all the Dashboards examples. This wouldn't happen in a real live app!</small>
                            </h6>
                        </div>
                    </div>
                }>
                    <Route path="/dashboards" component={Dashboards} />
                </Suspense>

                <Route exact path="/" render={() => (
                    <Redirect to="/pages/landingpage" />
                )} />
                <ToastContainer />
            </Fragment>
        )
    }
}

// const AppMain = () => {

// };

export default AppMain;