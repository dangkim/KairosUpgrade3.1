import React, { Component } from "react";
import { connect } from 'react-redux';
import { infActions } from '../_actions';
import { history } from '../_helpers';


var NumberFormat = require('react-number-format');

class InfluencerUpdateCostPage extends Component {

    constructor(props) {
        super(props);

        this.state = {
            shareLink: '',
            postImage: '',
            video: '',
            liveStream: '',
            checkIn: '',
            submitted: false,
            influencer: props.influencers.items
        };

        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleSubmit(e) {
        e.preventDefault();

        this.setState({ submitted: true });
        const { influencer } = this.state;
        const { dispatch } = this.props;

        if (influencer.shareLink &&
            influencer.postImage &&
            influencer.liveStream &&
            influencer.checkIn &&
            influencer.video) {
            //let obj = JSON.parse(influencer.items);
            const { userName } = this.props.location.state;
            dispatch(infActions.updateInfluencers(influencer, userName));
        }
    }

    componentDidMount() {
        const { dispatch } = this.props;

        if (this.props.location.state) {
            const { userName } = this.props.location.state;
            dispatch(infActions.getCostByUserName(userName));
        }
    }

    render() {
        const userName = this.props.location.state;
        const { submitted, influencer } = this.state;
        const { loading } = this.props;
        return (
            <div>
                <TopHeaderPage />
                <div className="app-main">
                    <LeftMenuPage userName={userName} />
                    <div className="app-main__outer">
                        <div className="app-main__inner">
                            <div className="app-page-title">
                                <div className="page-title-wrapper">
                                    <div className="page-title-heading">
                                        <div className="page-title-icon">
                                            <i className="lnr-picture text-danger">
                                            </i>
                                        </div>
                                        <div>Update your value
                                    <div className="page-title-subheading">Modification is very easy to do using KolViet Framework.
                                    </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="main-card mb-3 card">
                                <div className="card-body">
                                    <form id="costForm" className="col-md-10 mx-auto">
                                        <div className="form-row">
                                            <div className="col-md-2">
                                                <label htmlFor="shareLink">Share Link</label>
                                                <div>
                                                    <NumberFormat className="form-control" id="shareLink" name="shareLink" thousandSeparator={true} suffix={'đ'} value={this.props.influencers.items ? this.props.influencers.items.shareLink : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { formattedValue, value } = values;
                                                        const influencer = this.props.influencers.items;
                                                        if (influencer) {
                                                            influencer.shareLink = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.shareLink &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </div>
                                            <div className="col-md-2">
                                                <label htmlFor="postImage">Post Image</label>
                                                <div>
                                                    <NumberFormat className="form-control" id="postImage" name="postImage" thousandSeparator={true} suffix={'đ'} value={this.props.influencers.items ? this.props.influencers.items.postImage : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { formattedValue, value } = values;
                                                        const influencer = this.props.influencers.items;
                                                        if (influencer) {
                                                            influencer.postImage = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.postImage &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </div>
                                            <div className="col-md-2">
                                                <label htmlFor="video">Video</label>
                                                <div>
                                                    <NumberFormat className="form-control" id="video" name="video" thousandSeparator={true} suffix={'đ'} value={this.props.influencers.items ? this.props.influencers.items.video : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { formattedValue, value } = values;
                                                        const influencer = this.props.influencers.items;
                                                        if (influencer) {
                                                            influencer.video = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.video &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </div>
                                            <div className="col-md-3">
                                                <label htmlFor="checkIn">Check In</label>
                                                <div>
                                                    <NumberFormat className="form-control" id="checkIn" name="checkIn" thousandSeparator={true} suffix={'đ'} value={this.props.influencers.items ? this.props.influencers.items.checkIn : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { formattedValue, value } = values;
                                                        const influencer = this.props.influencers.items;
                                                        if (influencer) {
                                                            influencer.checkIn = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.checkIn &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </div>
                                            <div className="col-md-3">
                                                <label htmlFor="liveStream">Live Stream</label>
                                                <div>
                                                    <NumberFormat className="form-control" id="liveStream" name="liveStream" thousandSeparator={true} suffix={'đ'} value={this.props.influencers.items ? this.props.influencers.items.liveStream : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { formattedValue, value } = values;
                                                        const influencer = this.props.influencers.items;
                                                        if (influencer) {
                                                            influencer.liveStream = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.liveStream &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </div>
                                        </div>
                                        <div className="divider row"></div>
                                        <div className="d-flex align-items-center">
                                            <div className="ml-auto">
                                                <button onClick={this.handleSubmit} className="btn btn-primary btn-lg">Confirm</button>
                                                {
                                                    (this.props.influencers.loading) &&
                                                    <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                                                }
                                            </div>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        );
    }
}

function mapStateToProps(state) {
    const { influencers } = state;
    return {
        influencers
    };
}

const connectedInfluencerUpdateCostPage = connect(mapStateToProps)(InfluencerUpdateCostPage);
export { connectedInfluencerUpdateCostPage as InfluencerUpdateCostPage };