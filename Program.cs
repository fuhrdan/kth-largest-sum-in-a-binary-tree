//*****************************************************************************
//** 2583. Kth Largest Sum in a Binary Tree   leetcode                       **
//*****************************************************************************


/**
 * Definition for a binary tree node.
 * struct TreeNode {
 *     int val;
 *     struct TreeNode *left;
 *     struct TreeNode *right;
 * };
 */
// Queue structure for level-order traversal
struct QueueNode {
    struct TreeNode *data;
    struct QueueNode *next;
};

struct Queue {
    struct QueueNode *front;
    struct QueueNode *rear;
};

struct Queue* createQueue() {
    struct Queue* q = (struct Queue*) malloc(sizeof(struct Queue));
    q->front = q->rear = NULL;
    return q;
}

int isQueueEmpty(struct Queue* q) {
    return (q->front == NULL);
}

void enqueue(struct Queue* q, struct TreeNode* node) {
    struct QueueNode* temp = (struct QueueNode*) malloc(sizeof(struct QueueNode));
    temp->data = node;
    temp->next = NULL;
    if (q->rear == NULL) {
        q->front = q->rear = temp;
        return;
    }
    q->rear->next = temp;
    q->rear = temp;
}

struct TreeNode* dequeue(struct Queue* q) {
    if (q->front == NULL)
        return NULL;
    struct QueueNode* temp = q->front;
    struct TreeNode* node = temp->data;
    q->front = q->front->next;
    if (q->front == NULL)
        q->rear = NULL;
    free(temp);
    return node;
}

// Min-heap structure for keeping the k largest sums
struct MinHeap {
    long long *heapArr;
    int size;
    int capacity;
};

struct MinHeap* createMinHeap(int capacity) {
    struct MinHeap* heap = (struct MinHeap*) malloc(sizeof(struct MinHeap));
    heap->size = 0;
    heap->capacity = capacity;
    heap->heapArr = (long long*) malloc(capacity * sizeof(long long));
    return heap;
}

void swap(long long *a, long long *b) {
    long long temp = *a;
    *a = *b;
    *b = temp;
}

void heapify(struct MinHeap* heap, int i) {
    int smallest = i;
    int left = 2 * i + 1;
    int right = 2 * i + 2;
    if (left < heap->size && heap->heapArr[left] < heap->heapArr[smallest])
        smallest = left;
    if (right < heap->size && heap->heapArr[right] < heap->heapArr[smallest])
        smallest = right;
    if (smallest != i) {
        swap(&heap->heapArr[i], &heap->heapArr[smallest]);
        heapify(heap, smallest);
    }
}

void insertMinHeap(struct MinHeap* heap, long long val) {
    if (heap->size == heap->capacity) {
        if (val > heap->heapArr[0]) {
            heap->heapArr[0] = val;
            heapify(heap, 0);
        }
    } else {
        heap->heapArr[heap->size] = val;
        int i = heap->size;
        heap->size++;
        while (i != 0 && heap->heapArr[i] < heap->heapArr[(i - 1) / 2]) {
            swap(&heap->heapArr[i], &heap->heapArr[(i - 1) / 2]);
            i = (i - 1) / 2;
        }
    }
}

long long getMin(struct MinHeap* heap) {
    if (heap->size == 0) return -1;
    return heap->heapArr[0];
}

long long kthLargestLevelSum(struct TreeNode* root, int k) {
    if (!root) return -1;

    struct Queue* q = createQueue();
    enqueue(q, root);

    struct MinHeap* heap = createMinHeap(k);

    while (!isQueueEmpty(q)) {
        int currentLevelNodeCount = 0;
        long long currentLevelSum = 0;
        
        // Calculate the current level node count
        struct QueueNode *temp = q->front;
        while (temp) {
            currentLevelNodeCount++;
            temp = temp->next;
        }

        for (int i = 0; i < currentLevelNodeCount; i++) {
            struct TreeNode* currentNode = dequeue(q);
            currentLevelSum += currentNode->val;

            if (currentNode->left) {
                enqueue(q, currentNode->left);
            }
            if (currentNode->right) {
                enqueue(q, currentNode->right);
            }
        }

        insertMinHeap(heap, currentLevelSum);
    }

    return (heap->size == k) ? getMin(heap) : -1;
}