import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';
import { findDOMNode } from 'react-dom'
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

import screenfull from 'screenfull'

import defaultAvatar from '../../../../assets/utils/images/avatars/default.jpg'
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
            dateValue: moment.range(today.clone(), today.clone().add(7, "days")),
            url: null,
            pip: false,
            playing: true,
            controls: false,
            light: false,
            volume: 0.8,
            muted: false,
            played: 0,
            loaded: 0,
            duration: 0,
            playbackRate: 1.0,
            loop: false
        };

        this.load = this.load.bind(this);
        this.handlePlayPause = this.handlePlayPause.bind(this);
        this.handleStop = this.handleStop.bind(this);
        this.handleToggleControls = this.handleToggleControls.bind(this);
        this.handleVolumeChange = this.handleVolumeChange.bind(this);
        this.handleToggleMuted = this.handleToggleMuted.bind(this);
        this.handlePause = this.handlePause.bind(this);
        this.handleClickFullscreen = this.handleClickFullscreen.bind(this);
        this.renderLoadButton = this.renderLoadButton.bind(this);
        this.handlePlay = this.handlePlay.bind(this);
    }

    load() {
        this.setState({
            url,
            played: 0,
            loaded: 0,
            pip: false
        })
    }

    handlePlayPause() {
        this.setState({ playing: !this.state.playing })
    }

    handleStop() {
        this.setState({ url: null, playing: false })
    }

    handleToggleControls() {
        const url = this.state.url
        this.setState({
            controls: !this.state.controls,
            url: null
        }, () => this.load(url))
    }

    handleVolumeChange() {
        this.setState({ playing: !this.state.playing })
    }

    handleToggleMuted() {
        this.setState({ muted: !this.state.muted })
    }

    handlePause() {
        console.log('onPause')
        this.setState({ playing: false })
    }

    handleClickFullscreen() {
        screenfull.request(findDOMNode(this.player))
    }

    renderLoadButton(url, label) {
        return (
            <button onClick={() => this.load(url)}>
                {label}
            </button>
        )
    }

    handlePlay = () => {
        console.log('onPlay')
        this.setState({ playing: true })
    }

    //componentDidMount() {
    //const { dispatch } = this.props;
    //const { first } = this.state;
    //dispatch(infActions.getAll(first, 0));
    //}

    ref = player => {
        this.player = player
    }

    render() {
        const { url, currentVideoIndex, playing, controls, light, volume, muted, loop, played, loaded, duration, playbackRate, pip } = this.state
        const { Influencer } = this.props;

        debugger;

        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <div>
                            <Row>
                                <Col md="12">
                                    <Card className="main-card mb-3">
                                        <div className="card-header">{Influencer ? Influencer.fullName : ''}
                                        </div>
                                        <ReactPlayer
                                            ref={this.ref}
                                            className='react-player'
                                            width='100%'
                                            height='100%'
                                            url={url}
                                            // url={Influencer ? Influencer.videoLink.paths[currentVideoIndex] : ''}
                                            pip={pip}
                                            playing={playing}
                                            controls={controls}
                                            light={light}
                                            loop={loop}
                                            playbackRate={playbackRate}
                                            volume={volume}
                                            muted={muted}
                                            onReady={() => console.log('onReady')}
                                            onStart={() => console.log('onStart')}
                                            onPlay={this.handlePlay}
                                            onPause={this.handlePause}
                                            onBuffer={() => console.log('onBuffer')}
                                            onSeek={e => console.log('onSeek', e)}
                                            onError={e => console.log('onError', e)}
                                        />
                                        <div className="d-block text-center card-footer">
                                            <Row>
                                                <Col md={2}>
                                                    <button onClick={this.handlePlayPause}>{playing ? 'Pause' : 'Play'}</button>
                                                    <button onClick={this.handleClickFullscreen}>Fullscreen</button>
                                                </Col>
                                                <Col md={2}>
                                                    <input id='controls' type='checkbox' checked={controls} onChange={this.handleToggleControls} />
                                                    <em>&nbsp; Player reload</em>
                                                </Col>
                                                <Col md={2}>
                                                    <input id='muted' type='checkbox' checked={muted} onChange={this.handleToggleMuted} />
                                                    <em>&nbsp; Muted</em>
                                                </Col>
                                            </Row>
                                            <Row>
                                                <Col md={1}>
                                                    Volume
                                                </Col>
                                                <Col md={1}>
                                                    <input type='range' min={0} max={1} step='any' value={volume} onChange={this.handleVolumeChange} />
                                                </Col>
                                            </Row>
                                            <Row>
                                                <Col md={4}>
                                                    <input ref={input => { this.urlInput = input }} type='text' placeholder='Enter URL' />
                                                    <button onClick={() => this.setState({ url: this.urlInput.value })}>Load</button>
                                                </Col>
                                            </Row>
                                        </div>
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