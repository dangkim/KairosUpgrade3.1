import React, { Fragment } from 'react';
import { connect } from 'react-redux';
import Tabs from 'react-responsive-tabs';

import PageTitleCategory from '../../../Layout/AppMain/PageTitleCategory';
import { history } from '../../../_helpers';
// Examples
import BasicExample from './Examples/Basic';
import { ColorsExample } from './Examples/Colors';
import { Influencers } from './Examples/Influencers';
import { CreateCampaign } from './Examples/CreateCampaign';
import { InfluencerDetail } from './Examples/InfluencerDetail';
import { InfluencerUpdateCost } from './Examples/InfluencerUpdateCost';
import { CompareInfluencers } from './Examples/CompareInfluencers';
import {
    Modal, ModalHeader, ModalBody, ModalFooter, Button, Row, Col, Card, CardBody
} from 'reactstrap';
import ScrollUpButton from "react-scroll-up-button";
import { Prompt } from 'react-router'
import { fab } from '@fortawesome/free-brands-svg-icons'
import {
    faHamburger,
    faMortarPestle,
    faRunning,
    faPlaneDeparture,
    faMicrophoneAlt,
    faStoreAlt,
    faMicrochip,
    faLandmark,
    faCouch,
    faBlender,
    faGamepad,
    faCar,
    faTshirt,
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { infActions } from '../../../_actions';

class WidgetsChartBoxes extends React.Component {

    constructor(props) {
        super(props);

        let brandLocal = null;
        let type = '';
        let userName = '';
        if (this.props.location.state) {
            if (this.props.location.state.Brand) {
                if (this.props.location.state.Brand[0]) {
                    brandLocal = this.props.location.state.Brand[0];
                }
                else {
                    brandLocal = this.props.location.state.Brand;
                }
            }

            if (this.props.location.state.type) {
                type = this.props.location.state.type;
                userName = this.props.location.state.userName;
            }

            if (this.props.location.state.userName) {
                userName = this.props.location.state.userName;
            }

        }
        this.state = {
            Brand: brandLocal,
            userName: userName,
            type: type,
            selectedTabKey: 0,
            Influencer: null,
            ComparedInfluencers: [],
            confirmedNavigation: false,
            modalVisible: false,
            FilterInfluencers: [],
            first: 9,
            SearchValue: '',
            cSelected: [],
            lastLocation: "/widgets/dashboard-boxes",
        };

        this.callbackFunction = this.callbackFunction.bind(this);
        this.handleConfirmNavigationClick = this.handleConfirmNavigationClick.bind(this);
        this.handleBlockedNavigation = this.handleBlockedNavigation.bind(this);
        this.showModal = this.showModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.filterCategory = this.filterCategory.bind(this);
        this.onCheckboxBtnClick = this.onCheckboxBtnClick.bind(this);
        this.onChangeTab = this.onChangeTab.bind(this);
    }

    onChangeTab(activeTab) {
        this.setState({ selectedTabKey: activeTab })
    }

    filterCategory(event) {
        const { first } = this.state;
        const { dispatch } = this.props;
        const searchValue = (event.target.value);
        this.setState({ SearchValue: searchValue });

        dispatch(infActions.getInfluencersByName(first, 0, searchValue));
    }

    callbackFunction = (selectedTabKey, childData, index) => {

        if (index !== null) {
            this.setState({ selectedTabKey: selectedTabKey, Influencer: childData[index] })
        }
        else {
            this.setState({ selectedTabKey: selectedTabKey, ComparedInfluencers: childData })
        }
    }

    showModal = location => {
        this.setState({
            modalVisible: true,
            lastLocation: location
        })
    };

    closeModal = (callback = () => { }) =>
        this.setState(
            {
                modalVisible: false
            },
            callback
        );

    handleBlockedNavigation = nextLocation => {
        const { confirmedNavigation } = this.state;
        if (!confirmedNavigation) {
            this.showModal(nextLocation);
            return false;
        }

        return true;
    };

    handleConfirmNavigationClick = () =>
        this.closeModal(() => {
            const { lastLocation } = this.state;
            if (lastLocation) {
                this.setState(
                    {
                        confirmedNavigation: true
                    },
                    () => {
                        //const startPageIndex = history.length - 1;
                        //history.index = -1;
                        // Navigate to the previous blocked location with your navigate function
                        //history.push(lastLocation.pathname);
                        //history.go(-startPageIndex);
                        history.replace({ pathname: lastLocation.pathname });
                    }
                );
            }
        });

    onCheckboxBtnClick(selected1, selected2) {
        const { cSelected, first, SearchValue } = this.state;
        const { dispatch } = this.props;
        let index = cSelected.indexOf(selected1);
        if (index < 0) {
            cSelected.push(selected1);
        } else {
            cSelected.splice(index, 1);
        }

        if (selected2 !== '') {
            index = cSelected.indexOf(selected2);
            if (index < 0) {
                cSelected.push(selected2);
            } else {
                cSelected.splice(index, 1);
            }
        }

        this.setState({ cSelected: [...cSelected] });
        this.setState({ selectedTabKey: 0 });
        let items = [];
        debugger;
        if (cSelected.length > 0) {
            cSelected.map((item, key) => {
                items.push(item);
            })
        }

        if (SearchValue !== '') {
            items.push(SearchValue);
        }

        dispatch(infActions.getInfluencersByCategory([], first, 0, items));
    }

    render() {
        const { cSelected, Influencer, Brand, modalVisible, type, userName, ComparedInfluencers } = this.state;
        const { FilterInfluencers, SearchValue } = this.props;

        const tabsContentUpdateCost = [
            {
                title: 'Vertical Menus',
                content: <InfluencerUpdateCost userName={userName} />
            }
        ]
        const tabsContent = [
            {
                title: 'Influencers',
                content: <Influencers SearchValue={SearchValue} FilterInfluencers={FilterInfluencers} parentCallback={this.callbackFunction} />
            },
            {
                title: 'Influencer details',
                content: <InfluencerDetail Influencer={Influencer} />
            },
            {
                title: 'Comparison Influencers',
                content: <CompareInfluencers ComparedInfluencers={ComparedInfluencers} />
            }
        ]

        const clickedStyle = {
            color: '#B1BBC4',
            cursor: 'pointer',
        };

        const noClickedStyle = {
            cursor: 'pointer',
        };

        const getTabs = () => {

            if (type === "Influencer") {
                return (tabsContentUpdateCost.map((tab, index) => ({
                    title: tab.title,
                    getContent: () => tab.content,
                    key: index,
                })))
            }
            else {
                return (tabsContent.map((tab, index) => ({
                    title: tab.title,
                    getContent: () => tab.content,
                    key: index,
                })))
            }
        }

        return (
            <Fragment>
                <Prompt when={!type || type !== "Influencer"} message={this.handleBlockedNavigation} />
                <Modal isOpen={modalVisible}>
                    <ModalHeader></ModalHeader>
                    <ModalBody>
                        Are you sure you want to leave?
                    </ModalBody>
                    <ModalFooter>
                        <Button color="link" onClick={() => this.closeModal(() => { })}>Cancel</Button>
                        <Button color="primary" onClick={this.handleConfirmNavigationClick}>Yes</Button>{' '}
                    </ModalFooter>
                </Modal>
                <Tabs onChange={this.onChangeTab} selectedTabKey={this.state.selectedTabKey} tabsWrapperClass="body-tabs body-tabs-layout" transform={false} showInkBar={true} items={getTabs()} />
                <ScrollUpButton />
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

const connectedWidgetsChartBoxes = connect(mapStateToProps)(WidgetsChartBoxes);
export { connectedWidgetsChartBoxes as WidgetsChartBoxes };