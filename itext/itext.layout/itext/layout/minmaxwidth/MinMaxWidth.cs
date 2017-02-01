using System;

namespace iText.Layout.Minmaxwidth {
    public class MinMaxWidth {
        private float childrenMinWidth;

        private float childrenMaxWidth;

        private float additionalWidth;

        private float availableWidth;

        public MinMaxWidth(float additionalWidth, float availableWidth)
            : this(additionalWidth, availableWidth, 0, 0) {
        }

        public MinMaxWidth(float additionalWidth, float availableWidth, float childrenMinWidth, float childrenMaxWidth
            ) {
            this.childrenMinWidth = childrenMinWidth;
            this.childrenMaxWidth = childrenMaxWidth;
            this.additionalWidth = additionalWidth;
            this.availableWidth = availableWidth;
        }

        public virtual float GetChildrenMinWidth() {
            return childrenMinWidth;
        }

        public virtual void SetChildrenMinWidth(float childrenMinWidth) {
            this.childrenMinWidth = childrenMinWidth;
        }

        public virtual float GetChildrenMaxWidth() {
            return childrenMaxWidth;
        }

        public virtual void SetChildrenMaxWidth(float childrenMaxWidth) {
            this.childrenMaxWidth = childrenMaxWidth;
        }

        public virtual float GetAdditionalWidth() {
            return additionalWidth;
        }

        public virtual void SetAdditionalWidth(float additionalWidth) {
            this.additionalWidth = additionalWidth;
        }

        public virtual float GetMaxWidth() {
            return Math.Min(childrenMaxWidth + additionalWidth, availableWidth);
        }

        public virtual float GetMinWidth() {
            return Math.Min(childrenMinWidth + additionalWidth, GetMaxWidth());
        }
    }
}
