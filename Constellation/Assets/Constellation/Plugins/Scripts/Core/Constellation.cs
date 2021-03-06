﻿using System.Collections.Generic;

namespace Constellation
{
    [System.Serializable]
    public class Constellation : ConstellationObject
    {
        public static ConstellationEventSystem eventSystem;
        private List<Node<INode>> Nodes;
        public List<Link> Links;
        public delegate void NodeAdded(Node<INode> node, NodeData nodeData);
        protected NodesFactory NodesFactory;

        public Constellation(ConstellationScriptData constellationScriptData, 
            NodesFactory nodesFactory, 
            NodeAdded onNodeAdded = null)
        {
            NodesFactory = nodesFactory;
            SetNodes(constellationScriptData.GetNodes(), onNodeAdded);
            SetLinks(constellationScriptData.GetLinks());
        }

        void SetNodes(NodeData[] nodes, NodeAdded onNodeAdded)
        {
            foreach (NodeData node in nodes)
            {
                var newNode = NodesFactory.GetNode(node);
                AddNode(newNode, node.Guid, node);
                if(onNodeAdded != null)
                    onNodeAdded(newNode, node);
            }
        }

        void SetLinks(LinkData [] links)
        {
            foreach (LinkData link in links)
            {
                var input = GetInput(link.Input.Guid);
                var output = GetOutput(link.Output.Guid);
                if (input != null && output != null)
                    AddLink(new Link(GetInput(link.Input.Guid),
                        GetOutput(link.Output.Guid),
                        GetOutput(link.Output.Guid).Type, link.GUID));
            }
        }

        public override void Initialize(string _guid, string _name)
        {
            if (Constellation.eventSystem == null)
                eventSystem = new ConstellationEventSystem();
            base.Initialize(_guid, _name);
            Injector = new Injector(this);
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            SetConstellationEvents();
        }

        public void SetConstellationEvents()
        {
            Injector.SetConstellationEvents();
        }

        public void AddLink(LinkData link)
        {
            AddLink(new Link(GetInput(link.Input.Guid),
                GetOutput(link.Output.Guid),
                GetOutput(link.Output.Guid).Type, link.GUID));
        }

        public Input GetInput(string guid)
        {
            foreach (Node<INode> node in Nodes)
            {
                if (node.Inputs != null)
                {
                    foreach (Input input in node.Inputs)
                    {

                        if (guid == input.Guid)
                        {
                            return input;
                        }
                    }
                }
            }
            //Debug.Log(guid + " not found for Input");
            return null;
        }

        public Node<INode> GetNodeByGUID(string guid)
        {
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            foreach(var node in Nodes)
            {
                if(node.Guid == guid)
                {
                    return node;
                }
            }
            return null;
        }


        public Node<INode>[] GetNodes()
        {
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            return Nodes.ToArray();
        }

        public Output GetOutput(string guid)
        {
            foreach (Node<INode> node in Nodes)
            {
                if (node.Outputs != null)
                {
                    foreach (Output output in node.Outputs)
                    {
                        if (guid == output.Guid)
                        {
                            return output;
                        }
                    }
                }
            }
            //Debug.Log(guid + " not found for Output");
            return null;
        }

        public Node<INode> AddNode(Node<INode> node, string guid, NodeData nodeData = null)
        {
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            var newNode = node;
            newNode.Initialize(guid, node.Name);
            if (nodeData != null)
            {
                newNode.XPosition = nodeData.XPosition;
                newNode.YPosition = nodeData.YPosition;
                newNode.XSize = nodeData.SizeX;
                newNode.YSize = nodeData.SizeY;
            }
            Nodes.Add(newNode);
            if(Injector != null)
                Injector.RefreshConstellationEvents();

            if (newNode.NodeType is ICustomNode)
            {
                (newNode.NodeType as ICustomNode).InitializeConstellation(NodesFactory.GetStaticConstellationScripts());
            }
            return newNode;
        }

        public Link[] GetLinks()
        {
            if (Links == null)
                Links = new List<Link>();

            return Links.ToArray();
        }

        public void RemovedNode(string guid)
        {
            foreach (var node in Nodes)
            {
                if (node.Guid == guid)
                {
                    var links = Links.ToArray();
                    foreach (var link in links)
                    {
                        foreach (var input in node.Inputs)
                        {
                            if (link.Input.Guid == input.Guid)
                            {
                                link.OnDestroy();
                                Links.Remove(link);
                            }
                        }
                        foreach (var output in node.Outputs)
                        {
                            if (link.Output.Guid == output.Guid)
                            {
                                link.OnDestroy();
                                Links.Remove(link);
                            }
                        }
                    }
                    node.Destroy();
                    Nodes.Remove(node);
                    Injector.RefreshConstellationEvents();
                    return;
                }
            }
            //Debug.LogError("Constellation: Node not found");
        }

        public Link AddLink(Link link)
        {
            if (Links == null)
                Links = new List<Link>();

            var newLink = link;
            link.Initialize(link.GUID, link.GUID);
            Links.Add(link);

            return newLink;
        }
    }
}