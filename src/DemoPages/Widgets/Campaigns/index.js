import React, { Fragment } from 'react';
import { Route } from 'react-router-dom';
import { connect } from 'react-redux';
import { campaignActions } from '../../../_actions';
import "@devexpress/dx-react-grid";
import {
    Grid,
    Table,
    TableHeaderRow
} from "@devexpress/dx-react-grid-material-ui";

import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';

import {
    Button, Form,
    FormGroup, Label,
    Row, Col,
    Card, CardBody,
    CardTitle,
} from 'reactstrap';

export default class CampaignsTable extends React.Component {

    constructor(props) {
        super(props);

        this.state = {
            compaignList: [],
            columns: [
                { name: "campaignName", title: "Name" },
                { name: "budget", title: "Budget" },
                { name: "campaignTarget", title: "Target" },
                { name: "description", title: "Description" },
                { name: "fromAge", title: "From Age" },
                { name: "toAge", title: "To Age" },
                { name: "fromDate", title: "From Date" },
                { name: "toDate", title: "To Date" },
                // { name: "hashTag", title: "HashTag" },
                { name: "gender", title: "Gender" },
                { name: "jobName", title: "JobName" },
                // { name: "keyword", title: "Keyword" },
                // { name: "link", title: "Link" },
                { name: "influencerFullName", title: "Influencer" },
            ],
        };

    }

    componentDidMount() {
        const { dispatch } = this.props;
        const brandName = localStorage.getItem("brandName");
        dispatch(campaignActions.getCampaignByBrand(brandName));
    }

    render() {
        const { columns } = this.state;
        const { campaign } = this.props;
        var rows = []

        const tableColumnExtensions = [
            { columnName: "campaignName", align: 'left', wordWrapEnabled: true },
            { columnName: "budget", align: 'left', wordWrapEnabled: true },
            { columnName: "campaignTarget", align: 'left', wordWrapEnabled: true },
            { columnName: "description", align: 'left', wordWrapEnabled: true },
            { columnName: "fromAge", align: 'left', wordWrapEnabled: true },
            { columnName: "toAge", align: 'left', wordWrapEnabled: true },
            { columnName: "fromDate", align: 'left', wordWrapEnabled: true },
            { columnName: "toDate", align: 'left', wordWrapEnabled: true },
            // { columnName: "hashTag", align: 'left', wordWrapEnabled: true },
            { columnName: "gender", align: 'left', wordWrapEnabled: true },
            { columnName: "jobName", align: 'left', wordWrapEnabled: true },
            // { columnName: "keyword", align: 'left', wordWrapEnabled: true },
            // { columnName: "link", align: 'left', wordWrapEnabled: true },
            { columnName: "influencerFullName", align: 'left', wordWrapEnabled: true }
        ]

        if (campaign) {
            rows = campaign.map((item, index) => {
                return Object.assign(item, {
                    campaignName: item.campaignName,
                    budget: item.budget,
                    campaignTarget: item.campaignTarget,
                    description: item.description,
                    fromAge: item.fromAge,
                    toAge: item.toAge,
                    fromDate: item.fromDate,
                    toDate: item.toDate,
                    hashTag: item.hashTag,
                    gender: item.gender,
                    jobName: item.jobName,
                    keyword: item.keyword,
                    link: item.link,
                    influencerFullName: item.bag.contentItems[0].fullName,
                });
            })
        }

        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Row className="text-center">
                            <Col md="12">
                                <Card className="main-card mb-3">
                                    <CardBody>
                                        <CardTitle>
                                            All Campaigns
                                    </CardTitle>
                                        <Grid rows={rows} columns={columns}>
                                            <Table columnExtensions={tableColumnExtensions}/>
                                            <TableHeaderRow />
                                        </Grid>
                                        <div className="divider" />
                                    </CardBody>
                                </Card>
                            </Col>

                        </Row></CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
}

function mapStateToProps(state) {

    const { campaign } = state.campaign;
    //const { brand } = influencers;
    return {
        campaign,
    };
}

const connectedCampaignsTable = connect(mapStateToProps)(CampaignsTable);
export { connectedCampaignsTable as CampaignsTable };