import React, { Fragment } from 'react';
import { Route } from 'react-router-dom';
import { connect } from 'react-redux';
// Dashboard Widgets
import { infActions } from '../../_actions';
import { WidgetsChartBoxes } from "./ChartBoxes/";

// Layout
import { AppHeader } from '../../Layout/AppHeader';
import AppSidebar from '../../Layout/AppSidebar/';
import AppFooter from '../../Layout/AppFooter/';
import { CampaignsTable } from './Campaigns';

class Widgets extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            searchValue: '',
            first: 9
        };

        this.sendData = this.sendData.bind(this);
    }

    sendData = (childData) => {
        const { dispatch } = this.props;
        if (childData && childData !== '') {

            let items = [];
            debugger;

            items.push(childData);

            dispatch(infActions.getInfluencersByCategory([], this.state.first, 0, items));
            //this.props.dispatch(infActions.getInfluencersByName(this.state.first, 0, childData));
            //this.setState({ searchValue: childData })
        }
    }

    render() {
        const { searchValue } = this.state;
        const { influencers } = this.props;
        const influencerItems = (influencers && influencers.items) ? influencers.items : [];

        return (<Fragment>
            <AppHeader parentCallback={this.sendData} />
            <div className="app-main">
                <AppSidebar />
                <div className="app-main__outer">
                    <div className="app-main__inner">

                        {/* Widgets */}

                        <Route path={`${this.props.match.url}/dashboard-boxes`}
                            render={(routeProps) => (
                                <WidgetsChartBoxes FilterInfluencers={influencerItems} SearchValue={searchValue} {...this.props} />
                            )} />
                        <Route path={`${this.props.match.url}/campaigns-table`}
                            render={(routeProps) => (
                                <CampaignsTable {...this.props} />
                            )} />
                    </div>
                </div>
            </div>
        </Fragment>
        );
    }
}

function mapStateToProps(state) {

    const { campaigns, influencers, locations, interestings, jobCategories, jobs, brands } = state;
    //const { brand } = influencers;
    return {
        //loggingIn,
        brands,
        jobs,
        jobCategories,
        interestings,
        locations,
        campaigns,
        influencers
    };
}

const connectedWidgets = connect(mapStateToProps)(Widgets);
export default connectedWidgets;

//export default Widgets;