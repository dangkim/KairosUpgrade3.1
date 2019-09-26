import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';
import NumberFormat from 'react-number-format';

import {
    Button, Form,
    FormGroup, Label,
    Row, Col,
    Card, CardBody,
    CardTitle,
} from 'reactstrap';


import { infActions } from '../../../../_actions';

import { history } from '../../../../_helpers';

class InfluencerUpdateCost extends Component {

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
        debugger;
        this.setState({ submitted: true });
        const { influencer } = this.state;
        const { dispatch, userName } = this.props;

        if (influencer.shareLink &&
            influencer.postImage &&
            influencer.liveStream &&
            influencer.checkIn &&
            influencer.video) {
            //let obj = JSON.parse(influencer.items);
            dispatch(infActions.updateInfluencers(influencer, userName));
        }
    }

    componentDidMount() {
        const { dispatch, userName } = this.props;
        dispatch(infActions.getCostByUserName(userName));
    }

    render() {

        const { submitted, influencer } = this.state;
        //let influencerProps = null;
        // if (this.props.influencers.influencer) {
        //     influencerProps = {
        //         shareLink: this.props.influencers.influencer.shareLink? this.props.influencers.influencer.shareLink: this.props.influencers.influencer.ShareLink.Text,
        //         postImage: this.props.influencers.influencer.postImage? this.props.influencers.influencer.postImage: this.props.influencers.influencer.PostImage.Text,
        //         video: this.props.influencers.influencer.video? this.props.influencers.influencer.video: this.props.influencers.influencer.Video.Text,
        //         checkIn: this.props.influencers.influencer.checkIn? this.props.influencers.influencer.checkIn: this.props.influencers.influencer.CheckIn.Text,
        //         liveStream: this.props.influencers.influencer.liveStream? this.props.influencers.influencer.liveStream: this.props.influencers.influencer.LiveStream.Text
        //     }
        // }
        const influencerProps = this.props.influencers.influencer;
        debugger;
        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Card className="main-card mb-3">
                            <CardBody>
                                <CardTitle>
                                    {
                                        <span>Update your value <span className="text-success">Modification is very easy to do using KolViet Framework</span></span>
                                    }
                                </CardTitle>
                                <Form id="updateCost">
                                    <Row form>
                                        <Col md={2}>
                                            <FormGroup>
                                                <Label for="ShareLink" className=""> <span className="text-danger">*</span> Share Link</Label>
                                                <div>
                                                    <NumberFormat className="form-control" id="shareLink" name="shareLink" thousandSeparator={true} suffix={'đ'} value={influencerProps ? influencerProps.shareLink : ''} placeholder="Price..." onValueChange={(values) => {
                                                        const { value } = values;
                                                        const influencer = this.props.influencers.influencer;
                                                        if (influencer) {
                                                            influencer.shareLink = value;
                                                            this.setState({ influencer: influencer })
                                                        }
                                                    }} />
                                                    {submitted && !influencer.shareLink &&
                                                        <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                    }
                                                </div>
                                            </FormGroup>
                                        </Col>
                                        <Col md={2}>
                                            <FormGroup>
                                                <Label for="postImage" className="">
                                                    <span className="text-danger">*</span> Post Image
                                                        </Label>
                                                <NumberFormat className="form-control" id="postImage" name="postImage" thousandSeparator={true} suffix={'đ'} value={influencerProps ? influencerProps.postImage : ''} placeholder="Price..." onValueChange={(values) => {
                                                    const { value } = values;
                                                    const influencer = this.props.influencers.influencer;
                                                    if (influencer) {
                                                        influencer.postImage = value;
                                                        this.setState({ influencer: influencer })
                                                    }
                                                }} />
                                                {submitted && !influencer.postImage &&
                                                    <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                }
                                            </FormGroup>
                                        </Col>
                                        <Col md={2}>
                                            <FormGroup>
                                                <Label for="video" className="">
                                                    <span className="text-danger">*</span> Video
                                                        </Label>
                                                <NumberFormat className="form-control" id="video" name="video" thousandSeparator={true} suffix={'đ'} value={influencerProps ? influencerProps.video : ''} placeholder="Price..." onValueChange={(values) => {
                                                    const { value } = values;
                                                    const influencer = this.props.influencers.influencer;
                                                    if (influencer) {
                                                        influencer.video = value;
                                                        this.setState({ influencer: influencer })
                                                    }
                                                }} />
                                                {submitted && !influencer.video &&
                                                    <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                }
                                            </FormGroup>
                                        </Col>
                                        <Col md={3}>
                                            <FormGroup>
                                                <Label for="checkIn" className="">
                                                    <span className="text-danger">*</span> Check In</Label>
                                                <NumberFormat className="form-control" id="checkIn" name="checkIn" thousandSeparator={true} suffix={'đ'} value={influencerProps ? influencerProps.checkIn : ''} placeholder="Price..." onValueChange={(values) => {
                                                    const { value } = values;
                                                    const influencer = this.props.influencers.influencer;
                                                    if (influencer) {
                                                        influencer.checkIn = value;
                                                        this.setState({ influencer: influencer })
                                                    }
                                                }} />
                                                {submitted && !influencer.checkIn &&
                                                    <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                }
                                            </FormGroup>
                                        </Col>
                                        <Col md={3}>
                                            <FormGroup>
                                                <Label for="liveStream" className="">
                                                    <span className="text-danger">*</span> Live Stream</Label>
                                                <NumberFormat className="form-control" id="liveStream" name="liveStream" thousandSeparator={true} suffix={'đ'} value={influencerProps ? influencerProps.liveStream : ''} placeholder="Price..." onValueChange={(values) => {
                                                    const { value } = values;
                                                    const influencer = this.props.influencers.influencer;
                                                    if (influencer) {
                                                        influencer.liveStream = value;
                                                        this.setState({ influencer: influencer })
                                                    }
                                                }} />
                                                {submitted && !influencer.liveStream &&
                                                    <div className="help-block" style={{ color: 'red' }}>Price is required</div>
                                                }
                                            </FormGroup>
                                        </Col>
                                    </Row>
                                    <Button onClick={this.handleSubmit} disabled={(this.props.influencers.loading)} color="primary" className="mt-2">Confirm</Button>
                                    {
                                        (this.props.influencers.loading) &&
                                        <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                                    }
                                </Form>
                            </CardBody>
                        </Card>
                    </CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
}


function mapStateToProps(state) {
    const { influencers } = state;
    return {
        influencers
    };
}

const connectedInfluencerUpdateCost = connect(mapStateToProps)(InfluencerUpdateCost);
export { connectedInfluencerUpdateCost as InfluencerUpdateCost };