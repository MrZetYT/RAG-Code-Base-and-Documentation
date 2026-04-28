import React, { useState, useEffect } from 'react';
import CytoscapeComponent from 'react-cytoscapejs';
import { graphApi } from '../api/graphApi';
import './GraphViewer.css';

const GraphViewer = () => {
    const [elements, setElements] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const loadGraph = async () => {
        setLoading(true);
        setError(null);
        try {
            const { nodes, edges } = await graphApi.getProjectGraph();

            // Конвертируем в формат Cytoscape
            const cyElements = [
                ...nodes.map(node => ({
                    data: {
                        id: node.id,
                        label: node.label || node.id,
                        type: node.type,
                    },
                })),
                ...edges.map(edge => ({
                    data: {
                        source: edge.source,
                        target: edge.target,
                    },
                })),
            ];

            setElements(cyElements);
        } catch (err) {
            console.error('Failed to load graph:', err);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadGraph();
    }, []);

    const layout = {
        name: 'breadthfirst',
        fit: true,
        padding: 30,
        circle: false,
        spacingFactor: 1.2,
        animate: true,
    };

    const stylesheet = [
        {
            selector: 'node[type="file"]',
            style: {
                'background-color': '#4A90E2',
                'label': 'data(label)',
                'color': '#ffffff',
                'font-size': '12px',
                'font-weight': 'bold',
                'text-outline-color': '#1a1a2e',
                'text-outline-width': '2px',
                'shape': 'roundrectangle',
                'width': 'label',
                'padding': '10px',
                'border-width': '2px',
                'border-color': '#ffffff',
            },
        },
        {
            selector: 'node[type="class"]',
            style: {
                'background-color': '#7ED321',
                'label': 'data(label)',
                'color': '#1a1a2e',
                'font-size': '12px',
                'font-weight': 'bold',
                'text-outline-color': '#7ED321',
                'text-outline-width': '1px',
                'shape': 'roundrectangle',
                'width': 'label',
                'padding': '10px',
                'border-width': '2px',
                'border-color': '#ffffff',
            },
        },
        {
            selector: 'node[type="method"]',
            style: {
                'background-color': '#F5A623',
                'label': 'data(label)',
                'color': '#1a1a2e',
                'font-size': '11px',
                'font-weight': 'bold',
                'text-outline-color': '#F5A623',
                'text-outline-width': '1px',
                'shape': 'ellipse',
                'padding': '8px',
                'border-width': '2px',
                'border-color': '#ffffff',
            },
        },
        {
            selector: 'edge',
            style: {
                'width': 2,
                'line-color': '#888888',
                'target-arrow-color': '#888888',
                'target-arrow-shape': 'triangle',
                'curve-style': 'bezier',
                'arrow-scale': 1.5,
            },
        },
        {
            selector: ':selected',
            style: {
                'border-width': '3px',
                'border-color': '#ffffff',
                'line-color': '#ffffff',
                'target-arrow-color': '#ffffff',
            },
        },
        {
            selector: ':hover',
            style: {
                'line-color': '#ffffff',
                'target-arrow-color': '#ffffff',
                'transition-property': 'line-color, target-arrow-color',
                'transition-duration': '0.2s',
            },
        },
    ];

    if (loading) {
        return (
            <div className="graph-loading">
                <div className="spinner"></div>
                <p>Загрузка графа проекта...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="graph-error">
                <p>❌ Ошибка загрузки графа: {error}</p>
                <button onClick={loadGraph}>Повторить</button>
            </div>
        );
    }

    return (
        <div className="graph-viewer">
            <div className="graph-header">
                <h3>📊 Граф структуры проекта</h3>
                <button onClick={loadGraph} className="refresh-graph-btn">
                    🔄 Обновить
                </button>
                <div className="graph-legend">
                    <span className="legend-file">📄 Файл</span>
                    <span className="legend-class">🏛️ Класс</span>
                    <span className="legend-method">🔧 Метод</span>
                </div>
            </div>
            <div className="graph-container">
                {elements.length > 0 ? (
                    <CytoscapeComponent
                        elements={elements}
                        layout={layout}
                        stylesheet={stylesheet}
                        style={{ width: '100%', height: '100%', minHeight: '600px' }}
                        cy={(cy) => {
                            // Дополнительная настройка после инициализации
                            cy.on('tap', 'node', (evt) => {
                                const node = evt.target;
                                console.log('Clicked node:', node.data());
                            });
                        }}
                    />
                ) : (
                    <div className="graph-empty">
                        <p>Нет данных для отображения графа</p>
                        <p className="graph-hint">Загрузите файлы проекта через вкладку "Загрузка проекта"</p>
                    </div>
                )}
            </div>
        </div>
    );
};

export default GraphViewer;