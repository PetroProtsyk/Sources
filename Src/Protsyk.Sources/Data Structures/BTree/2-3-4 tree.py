from graphviz import Digraph, nohtml

class Tree234:

    root = None

    def __init__(self):
        root = None

    def addAll(self, values):
        for value in values:
            self.add(value)

    def add(self, value):
        if (self.root == None):
            self.root = Node234();
            self.root.data = [value]
        else:
            self.__addTo__(value, self.root)

    def __addTo__(self, value, node):
        if (node.order() == 4):
            v = node.data[1]

            n1 = Node234()
            n1.data = [node.data[0]]

            n2 = Node234()
            n2.data = [node.data[2]]

            if (node.children != None):
                if (len(node.children) > 1):
                   n1.children =  [node.children[0], node.children[1]]
                   n1.children[0].parent = n1
                   n1.children[1].parent = n1
                elif (len(node.children) > 0):
                   n1.children =  [node.children[0]]
                   n1.children[0].parent = n1

                if (len(node.children) > 3):
                   n2.children =  [node.children[2], node.children[3]]
                   n2.children[0].parent = n2
                   n2.children[1].parent = n2
                elif (len(node.children) > 2):
                   n2.children =  [node.children[2]]
                   n2.children[0].parent = n2

            if (node.parent == None):
                r = Node234()
                r.data = [v]
                r.children = [n1, n2]
                n1.parent = r
                n2.parent = r
                self.root = r
                node = r
            else:
                for i in range(node.parent.order()):
                    if ((i == len(node.parent.data)) or (value < node.parent.data[i])):
                        node.parent.data.insert(i, v)
                        del node.parent.children[i]
                        node.parent.children.insert(i, n1)
                        node.parent.children.insert(i+1, n2)
                        n1.parent = node.parent
                        n2.parent = node.parent
                        node = n2
                        break

        for i in range(node.order()):
            if ((i == len(node.data)) or (value < node.data[i])):
                if (node.isLeaf()):
                    node.data.insert(i, value)
                else:
                    self.__addTo__(value, node.children[i])
                break

    def renderDot(self):
        dot = Digraph(comment='Tree', node_attr={'shape': 'record'})
        if (self.root != None):
            self.render(dot, self.root, 0)
        return dot

    def render(self, dot, rootNode, label):
        if (rootNode.order() == 4):
            dot.node(str(label), nohtml('<r0> .|<v0> ' + str(rootNode.data[0]) + '|<r1> .|<v1> '+ str(rootNode.data[1]) +'|<r2> .|<v2> '+ str(rootNode.data[2]) +'|<r3> .'))
        if (rootNode.order() == 3):
            dot.node(str(label), nohtml('<r0> .|<v0> ' + str(rootNode.data[0]) + '|<r1> .|<v1> '+ str(rootNode.data[1]) +'|<r2> .'))
        if (rootNode.order() == 2):
            dot.node(str(label), nohtml('<r0> .|<v0> ' + str(rootNode.data[0]) + '|<r1> .'))

        rootLabel = label

        if (rootNode.children != None):
            for ci in range(len(rootNode.children)):
                dot.edge(str(rootLabel)+':r'+str(ci), str(label + 1))
                label = self.render(dot, rootNode.children[ci], label + 1)

        return label

class Node234:

    children = None
    data = None
    parent = None

    def __init__(self):
        children = None
        data = None
        parent = None

    def order(self):
        return 1 + len(self.data)

    def isLeaf(self):
        return self.children == None


words = ['a', 'b', 'c', 'p', 'q', 'r', 's', 'd', 't', 'e']

tst = Tree234()
tst.addAll(words)
tst.renderDot()