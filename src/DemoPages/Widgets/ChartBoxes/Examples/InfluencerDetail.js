import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';

import Slider from "react-slick";

import ReactPlayer from 'react-player';

import classnames from 'classnames';

import {
    Row, Col, ButtonGroup,
    Button,
    Card,
    CardHeader,
    CardBody,
} from 'reactstrap';

import defaultAvatar from '../../../../assets/utils/images/avatars/default.jpg'
import city from '../../../../assets//utils//images/originals/city.jpg'
import citynights from '../../../../assets/utils/images/originals/citynights.jpg'
import citydark from '../../../../assets/utils/images/originals/citydark.jpg'
import originalMoment from "moment";
import { extendMoment } from "moment-range";
import 'react-daterange-picker/dist/css/react-calendar.css'

class InfluencerDetail extends Component {
    constructor(props) {
        super(props);
        const moment = extendMoment(originalMoment);

        const today = moment();

        this.state = {
            searchValue: '',
            cSelected: [],
            currentVideoIndex: 0,
            dateValue: moment.range(today.clone(), today.clone().add(7, "days"))
        };

        this.nextVideo = this.nextVideo.bind(this);
    }

    nextVideo(event) {
        const { currentVideoIndex } = this.state;
        const nextVideoIndex = currentVideoIndex + Number(event.target.value);
        this.setState({ currentVideoIndex: nextVideoIndex });
    }

    //componentDidMount() {
    //const { dispatch } = this.props;
    //const { first } = this.state;
    //dispatch(infActions.getAll(first, 0));
    //}

    render() {
        const settings = {
            dots: true,
            infinite: true,
            fade: true,
            speed: 800,
            slidesToShow: 1,
            autoplay: true,
            slidesToScroll: 1
        };
        const { currentVideoIndex } = this.state;
        const { influencers, Influencer } = this.props;
        //const infItems = influencers.items ? influencers.items.influencer : [];
        let imgSrc = defaultAvatar;
        debugger;
        const colors = [
            "bg-mean-fruit",
            "bg-tempting-azure",
            "bg-amy-crisp",
            "bg-arielle-smile",
            "bg-happy-itmeo",
            "bg-strong-bliss",
            "bg-mean-fruit",
            "bg-tempting-azure",
            "bg-amy-crisp",
        ];
        const elements = ['1', '2', '3', '4', '5', '6'];
        const strOfReaction = Influencer ? Influencer.numberOfReaction : '';
        const strOfComment = Influencer ? Influencer.numberOfComment : '';
        const strOfShare = Influencer ? Influencer.numberOfShare : '';
        const numberOfReaction = strOfReaction.charAt(strOfReaction.length - 1) == 'k' ? Number((strOfReaction.substring(0, strOfReaction.length - 1))) * 1000 : Number(strOfReaction);
        const numberOfComment = strOfComment.charAt(strOfComment.length - 1) == 'k' ? Number((strOfComment.substring(0, strOfComment.length - 1))) * 1000 : Number(strOfComment);
        const numberOfShare = strOfShare.charAt(strOfShare.length - 1) == 'k' ? Number((strOfShare.substring(0, strOfShare.length - 1))) * 1000 : Number(strOfShare);
        const engagement = numberOfReaction + (numberOfComment * 2) + (numberOfShare * 3)

        const strOfReaction1 = Influencer ? Influencer.post1.numberOfReaction : '';
        const strOfComment1 = Influencer ? Influencer.post1.numberOfComment : '';
        const strOfShare1 = Influencer ? Influencer.post1.numberOfShare : '';
        const numberOfReaction1 = strOfReaction1.charAt(strOfReaction1.length - 1) == 'k' ? Number((strOfReaction1.substring(0, strOfReaction1.length - 1))) * 1000 : Number(strOfReaction1);
        const numberOfComment1 = strOfComment1.charAt(strOfComment1.length - 1) == 'k' ? Number((strOfComment1.substring(0, strOfComment1.length - 1))) * 1000 : Number(strOfComment1);
        const numberOfShare1 = strOfShare1.charAt(strOfShare1.length - 1) == 'k' ? Number((strOfShare1.substring(0, strOfShare1.length - 1))) * 1000 : Number(strOfShare1);
        const engagement1 = numberOfReaction1 + (numberOfComment1 * 2) + (numberOfShare1 * 3)

        const strOfReaction2 = Influencer ? Influencer.post2.numberOfReaction : '';
        const strOfComment2 = Influencer ? Influencer.post2.numberOfComment : '';
        const strOfShare2 = Influencer ? Influencer.post2.numberOfShare : '';
        const numberOfReaction2 = strOfReaction2.charAt(strOfReaction2.length - 1) == 'k' ? Number((strOfReaction2.substring(0, strOfReaction2.length - 1))) * 1000 : Number(strOfReaction2);
        const numberOfComment2 = strOfComment2.charAt(strOfComment2.length - 1) == 'k' ? Number((strOfComment2.substring(0, strOfComment2.length - 1))) * 1000 : Number(strOfComment2);
        const numberOfShare2 = strOfShare2.charAt(strOfShare2.length - 1) == 'k' ? Number((strOfShare2.substring(0, strOfShare2.length - 1))) * 1000 : Number(strOfShare2);
        const engagement2 = numberOfReaction2 + (numberOfComment2 * 2) + (numberOfShare2 * 3)

        const strOfReaction3 = Influencer ? Influencer.post3.numberOfReaction : '';
        const strOfComment3 = Influencer ? Influencer.post3.numberOfComment : '';
        const strOfShare3 = Influencer ? Influencer.post3.numberOfShare : '';
        const numberOfReaction3 = strOfReaction3.charAt(strOfReaction3.length - 1) == 'k' ? Number((strOfReaction3.substring(0, strOfReaction3.length - 1))) * 1000 : Number(strOfReaction3);
        const numberOfComment3 = strOfComment3.charAt(strOfComment3.length - 1) == 'k' ? Number((strOfComment3.substring(0, strOfComment3.length - 1))) * 1000 : Number(strOfComment3);
        const numberOfShare3 = strOfShare3.charAt(strOfShare3.length - 1) == 'k' ? Number((strOfShare3.substring(0, strOfShare3.length - 1))) * 1000 : Number(strOfShare3);
        const engagement3 = numberOfReaction3 + (numberOfComment3 * 2) + (numberOfShare3 * 3)

        const strOfReaction4 = Influencer ? Influencer.post4.numberOfReaction : '';
        const strOfComment4 = Influencer ? Influencer.post4.numberOfComment : '';
        const strOfShare4 = Influencer ? Influencer.post4.numberOfShare : '';
        const numberOfReaction4 = strOfReaction4.charAt(strOfReaction4.length - 1) == 'k' ? Number((strOfReaction4.substring(0, strOfReaction4.length - 1))) * 1000 : Number(strOfReaction4);
        const numberOfComment4 = strOfComment4.charAt(strOfComment4.length - 1) == 'k' ? Number((strOfComment4.substring(0, strOfComment4.length - 1))) * 1000 : Number(strOfComment4);
        const numberOfShare4 = strOfShare4.charAt(strOfShare4.length - 1) == 'k' ? Number((strOfShare4.substring(0, strOfShare4.length - 1))) * 1000 : Number(strOfShare4);
        const engagement4 = numberOfReaction4 + (numberOfComment4 * 2) + (numberOfShare4 * 3)

        const strOfReaction5 = Influencer ? Influencer.post5.numberOfReaction : '';
        const strOfComment5 = Influencer ? Influencer.post5.numberOfComment : '';
        const strOfShare5 = Influencer ? Influencer.post5.numberOfShare : '';
        const numberOfReaction5 = strOfReaction5.charAt(strOfReaction5.length - 1) == 'k' ? Number((strOfReaction5.substring(0, strOfReaction5.length - 1))) * 1000 : Number(strOfReaction5);
        const numberOfComment5 = strOfComment5.charAt(strOfComment5.length - 1) == 'k' ? Number((strOfComment5.substring(0, strOfComment5.length - 1))) * 1000 : Number(strOfComment5);
        const numberOfShare5 = strOfShare5.charAt(strOfShare5.length - 1) == 'k' ? Number((strOfShare5.substring(0, strOfShare5.length - 1))) * 1000 : Number(strOfShare5);
        const engagement5 = numberOfReaction5 + (numberOfComment5 * 2) + (numberOfShare5 * 3)

        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <div>
                            <Row>
                                <Col md="12" lg="6">
                                    <Row>
                                        <Col lg="6">
                                            <div className="card mb-3 widget-chart">
                                                <div className="widget-chart-content">
                                                    <div className="icon-wrapper rounded-circle">
                                                        <div className="icon-wrapper-bg bg-warning" />
                                                        <i className="lnr-thumbs-up text-warning" />
                                                    </div>
                                                    <div className="widget-numbers">
                                                        {Influencer ? Influencer.numberOfReaction : 0}
                                                    </div>
                                                    <div className="widget-subheading">
                                                        Reactions
                                                </div>
                                                </div>
                                            </div>
                                        </Col>
                                        <Col lg="6">
                                            <div className="card mb-3 widget-chart">
                                                <div className="widget-chart-content">
                                                    <div className="icon-wrapper rounded-circle">
                                                        <div className="icon-wrapper-bg bg-info" />
                                                        <i className="lnr-pencil text-info" />
                                                    </div>
                                                    <div className="widget-numbers">
                                                        {Influencer ? Influencer.numberOfComment : 0}
                                                    </div>
                                                    <div className="widget-subheading">
                                                        Comments
                                                </div>
                                                </div>
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                                <Col md="12" lg="6">
                                    <Row>
                                        <Col md="6">
                                            <div className="card mb-3 widget-chart">
                                                <div className="icon-wrapper rounded-circle">
                                                    <div className="icon-wrapper-bg bg-danger" />
                                                    <i className="lnr-link text-danger" />
                                                </div>
                                                <div className="widget-numbers">
                                                    {Influencer ? Influencer.numberOfShare : 0}
                                                </div>
                                                <div className="widget-subheading">
                                                    Share
                                            </div>
                                            </div>
                                        </Col>
                                        <Col md="6">
                                            <div className="card mb-3 widget-chart">
                                                <div className="icon-wrapper rounded-circle">
                                                    <div className="icon-wrapper-bg bg-success" />
                                                    <i className="lnr-license text-success" />
                                                </div>
                                                <div className="widget-numbers">
                                                    {engagement}
                                                </div>
                                                <div className="widget-subheading">
                                                    Engagement
                                            </div>
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col md="12">
                                    <Card className="main-card mb-3">
                                        <div className="card-header">{Influencer ? Influencer.fullName : ''}
                                        </div>
                                        <div className="table-responsive">
                                            <table className="align-middle mb-0 table table-borderless table-striped table-hover">
                                                <thead>
                                                    <tr>
                                                        <th className="text-center">Time</th>
                                                        <th>Status</th>
                                                        <th className="text-center">Reaction|Comment|Share</th>
                                                        <th className="text-center">Engagement</th>
                                                        <th className="text-center">Actions</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td className="text-center text-muted">{Influencer ? Influencer.post1.time : ''}</td>
                                                        <td>
                                                            <div className="widget-content p-0">
                                                                <div className="widget-content-wrapper">
                                                                    {/* <div className="widget-content-left mr-3">
                                                                        <div className="widget-content-left">
                                                                            <img width={40} className="rounded-circle" src={avatar4} alt="Avatar" />
                                                                        </div>
                                                                    </div> */}
                                                                    <div className="widget-content-left flex2">
                                                                        <div className="widget-heading">{Influencer ? Influencer.post1.title : ''}</div>
                                                                        <div className="widget-subheading opacity-7">{Influencer ? Influencer.post1.status : ''}</div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center">
                                                            <div className="badge badge-primary">{numberOfReaction1}</div>
                                                            <div className="badge badge-info">{numberOfComment1}</div>
                                                            <div className="badge badge-alternate">{numberOfShare1}</div>
                                                        </td>
                                                        <td className="text-center">{engagement1}</td>
                                                        <td className="text-center">
                                                            <button type="button" className="btn btn-primary btn-sm">Details</button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td className="text-center text-muted">{Influencer ? Influencer.post2.time : ''}</td>
                                                        <td>
                                                            <div className="widget-content p-0">
                                                                <div className="widget-content-wrapper">
                                                                    {/* <div className="widget-content-left mr-3">
                                                                        <div className="widget-content-left">
                                                                            <img width={40} className="rounded-circle" src={avatar4} alt="Avatar" />
                                                                        </div>
                                                                    </div> */}
                                                                    <div className="widget-content-left flex2">
                                                                        <div className="widget-heading">{Influencer ? Influencer.post2.title : ''}</div>
                                                                        <div className="widget-subheading opacity-7">{Influencer ? Influencer.post2.status : ''}</div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center">
                                                            <div className="badge badge-primary">{numberOfReaction2}</div>
                                                            <div className="badge badge-info">{numberOfComment2}</div>
                                                            <div className="badge badge-alternate">{numberOfShare2}</div>
                                                        </td>
                                                        <td className="text-center">{engagement2}</td>
                                                        <td className="text-center">
                                                            <button type="button" className="btn btn-primary btn-sm">Details</button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td className="text-center text-muted">{Influencer ? Influencer.post3.time : ''}</td>
                                                        <td>
                                                            <div className="widget-content p-0">
                                                                <div className="widget-content-wrapper">
                                                                    {/* <div className="widget-content-left mr-3">
                                                                        <div className="widget-content-left">
                                                                            <img width={40} className="rounded-circle" src={avatar4} alt="Avatar" />
                                                                        </div>
                                                                    </div> */}
                                                                    <div className="widget-content-left flex2">
                                                                        <div className="widget-heading">{Influencer ? Influencer.post3.title : ''}</div>
                                                                        <div className="widget-subheading opacity-7">{Influencer ? Influencer.post3.status : ''}</div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center">
                                                            <div className="badge badge-primary">{numberOfReaction3}</div>
                                                            <div className="badge badge-info">{numberOfComment3}</div>
                                                            <div className="badge badge-alternate">{numberOfShare3}</div>
                                                        </td>
                                                        <td className="text-center">{engagement3}</td>
                                                        <td className="text-center">
                                                            <button type="button" className="btn btn-primary btn-sm">Details</button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td className="text-center text-muted">{Influencer ? Influencer.post4.time : ''}</td>
                                                        <td>
                                                            <div className="widget-content p-0">
                                                                <div className="widget-content-wrapper">
                                                                    {/* <div className="widget-content-left mr-3">
                                                                        <div className="widget-content-left">
                                                                            <img width={40} className="rounded-circle" src={avatar4} alt="Avatar" />
                                                                        </div>
                                                                    </div> */}
                                                                    <div className="widget-content-left flex2">
                                                                        <div className="widget-heading">{Influencer ? Influencer.post4.title : ''}</div>
                                                                        <div className="widget-subheading opacity-7">{Influencer ? Influencer.post4.status : ''}</div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center">
                                                            <div className="badge badge-primary">{numberOfReaction4}</div>
                                                            <div className="badge badge-info">{numberOfComment4}</div>
                                                            <div className="badge badge-alternate">{numberOfShare4}</div>
                                                        </td>
                                                        <td className="text-center">{engagement4}</td>
                                                        <td className="text-center">
                                                            <button type="button" className="btn btn-primary btn-sm">Details</button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td className="text-center text-muted">{Influencer ? Influencer.post5.time : ''}</td>
                                                        <td>
                                                            <div className="widget-content p-0">
                                                                <div className="widget-content-wrapper">
                                                                    {/* <div className="widget-content-left mr-3">
                                                                        <div className="widget-content-left">
                                                                            <img width={40} className="rounded-circle" src={avatar4} alt="Avatar" />
                                                                        </div>
                                                                    </div> */}
                                                                    <div className="widget-content-left flex2">
                                                                        <div className="widget-heading">{Influencer ? Influencer.post5.title : ''}</div>
                                                                        <div className="widget-subheading opacity-7">{Influencer ? Influencer.post5.status : ''}</div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td className="text-center">
                                                            <div className="badge badge-primary">{numberOfReaction5}</div>
                                                            <div className="badge badge-info">{numberOfComment5}</div>
                                                            <div className="badge badge-alternate">{numberOfShare5}</div>
                                                        </td>
                                                        <td className="text-center">{engagement5}</td>
                                                        <td className="text-center">
                                                            <button type="button" className="btn btn-primary btn-sm">Details</button>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div className="d-block text-center card-footer">
                                        </div>
                                    </Card>
                                </Col>
                            </Row>
                            <Row>
                                <Col md="12">
                                    <Card className="main-card mb-3">
                                        <div className="card-header">Description
                                        </div>
                                        <div className="text-center" style={{ whiteSpace: 'pre-wrap' }}>
                                            {Influencer ? Influencer.description.split('|').join('\n') : ''}
                                        </div>
                                    </Card>
                                </Col>
                            </Row>
                            <Row>
                                <Col md="6">
                                    <Card className="main-card mb-3">
                                        <div className="card-header">Photos</div>
                                        <div className="slider-light">
                                            <div className="slick-slider">
                                                <div>
                                                    <Slider {...settings}>
                                                        {
                                                            Influencer && Influencer.photo.paths.map((url, index) => {
                                                                return (
                                                                    <div>
                                                                        <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-plum-plate" tabIndex="-1">
                                                                            <div className="slide-img-bg" style={{ backgroundImage: `url(${url})` }}></div>
                                                                        </div>
                                                                    </div>
                                                                )
                                                            })
                                                        }
                                                    </Slider>
                                                </div>
                                            </div>
                                        </div>
                                    </Card>
                                </Col>
                                <Col md="6">
                                    <Card className="main-card mb-3">
                                        <div className="card-header">Videos
                                        <div className="btn-actions-pane-right">
                                                <div role="group" className="btn-group-sm btn-group">
                                                    <Button onClick={this.nextVideo} value={-1} className="btn btn-info" disabled={currentVideoIndex === 0} >Back</Button>
                                                    <Button onClick={this.nextVideo} value={1} className="btn btn-info" disabled={currentVideoIndex === (Influencer && (Influencer.videoLink.paths.length - 1))}>Next</Button>
                                                </div>
                                            </div>
                                        </div>
                                        <ReactPlayer
                                            url={Influencer ? Influencer.videoLink.paths[currentVideoIndex] : ''}
                                            playing={false}
                                            width='100%'
                                            height='100%'
                                        />
                                    </Card>
                                </Col>
                            </Row>
                        </div>

                    </CSSTransition>
                </TransitionGroup>
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

const connectedInfluencerDetail = connect(mapStateToProps)(InfluencerDetail);
export { connectedInfluencerDetail as InfluencerDetail };
//export default Influencers;