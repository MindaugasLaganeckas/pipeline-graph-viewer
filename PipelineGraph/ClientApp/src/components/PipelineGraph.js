import React, {Component} from 'react';
import mermaid from 'mermaid';


export class PipelineGraph extends Component {
  
  constructor(props) {
    super(props);
    this.state = { pipelineGraph: "", loading: true };
  }
  
  componentDidMount() {
    this.populatePipelineGraphData();
  }

  static renderPipelineGraphTable(pipelineGraph) {
    const output = document.getElementById('output');
    mermaid.initialize({startOnLoad: true});
    mermaid.render('theGraph', pipelineGraph, function (svgCode) {
      output.innerHTML = svgCode;
    });
  }
  
  render() {
    let contents = "";
    if (this.state.loading || this.state.pipelineGrap === "") {
      contents = <p><em>No data received yet...</em></p>
    } else {
      PipelineGraph.renderPipelineGraphTable(this.state.pipelineGraph);
    }
    return (
        <div id="output"/>
    );
  }

  async populatePipelineGraphData() {
    const response = await fetch("pipelinegraph/" + this.props.match.params.id);
    const data = await response.json();
    this.setState({ pipelineGraph: data, loading: false });
  }
}

